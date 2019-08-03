using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using PokyPoker.Domain.Exceptions;

namespace PokyPoker.Domain
{
    public class Game
    {
        public Game(BettingRules rules, ImmutableArray<Player> players, Hand table, ImmutableArray<Round> rounds)
        {
            Rules = rules;
            Players = players;
            Table = table;
            Rounds = rounds;
        }

        public BettingRules Rules { get; }
        public ImmutableArray<Player> Players { get; }
        public Hand Table { get; }
        public ImmutableArray<Round> Rounds { get; }

        public IList<Player> ActivePlayers => Players.Where(p => p.IsActive).ToArray();

        public Stage Stage => (Stage) Rounds.Length;

        public Round CurrentRound => Rounds.Last();

        public Player Dealer => Players.Last();

        public Card[] CurrentTable => CropTable(Table, Stage);

        public bool IsComplete => (Stage == Stage.River && CurrentRound.IsComplete) || ActivePlayers.Count == 1;

        public Pot MainPot => SplitPot().Take(1).Single();

        public IEnumerable<Pot> SidePots => SplitPot().Skip(1);

        public Player CurrentPlayer => GetCurrentPlayer();

        public static Game StartNew(BettingRules rules, Player[] players, Deck deck)
        {
            var activePlayers = players
                .Select(p => new Player(p.Spot, deck.Take(2), true, p.Stack))
                .ToArray();

            var table = deck.Take(5);
            return StartNew(rules, activePlayers, table);
        }

        public static Game StartNew(BettingRules rules, Player[] players, Hand table)
        {
            if (players.Length < 2)
                throw new GameLogicException("Can't start round. Not enough players.");

            if (rules.BigBlind == 0)
                return new Game(rules, players.ToImmutableArray(), table,
                    ImmutableArray.Create(Round.StartNew(players)));

            players[0] = players[0].WithStack(s => s - rules.SmallBlind);
            players[1] = players[1].WithStack(s => s - rules.BigBlind);

            var acts = new[]
            {
                new Act(players[0], Play.Blind, rules.SmallBlind),
                new Act(players[1], Play.Blind, rules.BigBlind)
            };

            var round = new Round(acts, players);
            var rounds = ImmutableArray.Create(round);

            return new Game(rules, players.ToImmutableArray(), table, rounds);
        }

        private Game NextRound()
        {
            if (IsComplete)
                throw new GameLogicException("Can't start next round. Game is over.");

            if (!CurrentRound.IsComplete)
                throw new GameLogicException("Can't start next round. Current is incomplete.");

            var next = Round.StartNew(CurrentRound.ActivePlayers.ToArray());
            var rounds = Rounds.Add(next);

            return new Game(Rules, Players, Table, rounds);
        }


        public Game MakeAct(Play play, int bet = 0)
        {
            return MakeAct(CurrentPlayer, play, bet);
        }

        public Game MakeAct(Player player, Play play, int bet = 0)
        {
            if (IsComplete)
                throw new GameOrderException("Can not make act. Game is complete");

            if (player.Spot != CurrentPlayer.Spot)
                throw new GameOrderException($"Current player is {CurrentPlayer}");

            var options = GetOptions();
            if (!options.Contains(play))
                throw new GameLogicException($"Player is not supposed to make '{play}'");

            if (play == Play.Fold || play == Play.Check)
                bet = 0;

            if (play == Play.Raise && player.Stack == bet)
                play = Play.AllIn;


            player = player.MakePlay(play, bet, out var act);
            var rounds = Rounds.Replace(CurrentRound, CurrentRound.MakeAct(act));
            var players = Players.Replace(player, player);

            var game = new Game(Rules, players, Table, rounds);
            if (game.IsComplete)
                return new Game(Rules, GetResult(players), Table, rounds);

            return game.CurrentRound.IsComplete ? game.NextRound() : game;
        }

        public PlayerState GetPlayerState(Player player)
        {
            if (CurrentRound.ActivePlayers.All(p => p.Spot != player.Spot))
                return PlayerState.Inactive;

            var lastPlay = CurrentRound.LastPlay(player);
            return new PlayerState
            {
                Bet = CurrentRound.PlayerBet(player),
                IsActive = lastPlay != Play.Fold,
                ShouldAct = player == CurrentPlayer,
                LastPlay = lastPlay
            };
        }

        private Player GetCurrentPlayer()
        {
            var players = ActivePlayers
                .OrderBy(p => CurrentRound.LastPlay(p))
                .ThenBy(p => Players.IndexOf(p));

            foreach (var player in players)
            {
                if (CurrentRound.ShouldAct(player))
                    return player;
            }

            return Dealer;
        }

        public Play[] GetOptions()
        {
            if (CurrentRound.IsComplete)
                return Array.Empty<Play>();

            var options = CurrentRound
                .GetOptions(CurrentPlayer)
                .ToImmutableArray();

            if (CurrentRound.MaxBet >= CurrentPlayer.Stack)
            {
                options = options
                    .Replace(Play.Call, Play.AllIn)
                    .Remove(Play.Raise);
            }

            return options.ToArray();
        }

        private IEnumerable<Player> TakeContributors(IDictionary<Player, int> bets) => bets
            .Where(b => b.Value > 0)
            .Select(b => b.Key);

        private IEnumerable<Pot> SplitPot()
        {
            var acts = Rounds.SelectMany(r => r.Acts).ToArray();
            var playersBets = Players.ToDictionary(p => p, p => 0);

            var allInBets = new List<int>();
            foreach (var act in acts)
            {
                playersBets[act.Player] += act.Bet;
                if (act.Play == Play.AllIn)
                    allInBets.Add(playersBets[act.Player]);
            }

            if (!allInBets.Any())
            {
                var maxBet = playersBets.Max(p => p.Value);
                yield return Pot.Create(
                    playersBets.Where(p => p.Value == maxBet).Select(p => p.Key),
                    acts.Sum(a => a.Bet));
                yield break;
            }

            allInBets.Sort();
            var players = TakeContributors(playersBets).ToArray();

            var contribution = 0;
            foreach (var bet in allInBets)
            {
                var contenders = new HashSet<Player>();
                var amount = 0;
                contribution = bet - contribution;

                foreach (var player in players)
                {
                    if (playersBets[player] >= contribution)
                    {
                        contenders.Add(player);
                        playersBets[player] -= contribution;
                        amount += contribution;
                    }
                    else
                    {
                        amount += playersBets[player];
                        playersBets[player] = 0;
                    }
                }

                var pot = new Pot(amount, contenders);
                yield return pot;
            }

            yield return Pot.Create(
                TakeContributors(playersBets),
                playersBets.Sum(b => b.Value)
            );
        }

        public ImmutableArray<Player> GetResult(ImmutableArray<Player> players)
        {
            var pots = SplitPot();

            foreach (var pot in pots)
            {
                if (pot.Contenders.Count() == 1)
                {
                    var winner = pot.Contenders.Single();
                    players = players.Replace(winner, winner.WithStack(s => s + pot));
                    continue;
                }

                var winners = GetWinners(players.Where(p => p.IsActive)).ToList();
                var gain = pot / winners.Count;

                foreach (var winner in winners)
                {
                    players = players.Replace(winner, winner.WithStack(s => s + gain));
                }
            }

            return players;
        }

        private IEnumerable<Player> GetWinners(IEnumerable<Player> players)
        {
            var winners = new List<Player>();
            foreach (var player in players.Select(p => p.WithHand(h => h.Combine(Table))))
            {
                if (winners.Count == 0)
                {
                    winners.Add(player);
                    continue;
                }

                if (player.Hand > winners.First().Hand)
                {
                    winners.Clear();
                }
                else if (player.Hand < winners.First().Hand)
                {
                    continue;
                }

                winners.Add(player);
            }

            return winners;
        }


        private static Card[] CropTable(IEnumerable<Card> table, Stage stage)
        {
            switch (stage)
            {
                case Stage.PreFlop:
                    return new Card[0];
                case Stage.Flop:
                    return table.Take(3).ToArray();
                case Stage.Turn:
                    return table.Take(4).ToArray();
                case Stage.River:
                    return table.Take(5).ToArray();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum Stage
    {
        PreFlop = 1,
        Flop,
        Turn,
        River
    }
}
