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
        public Card[] CurrentTable => CropTable(Table, Stage);
        public bool IsComplete => Stage == Stage.River && CurrentRound.IsComplete;
        public Pot MainPot => SplitPot().Take(1).Single();
        public IEnumerable<Pot> SidePots => SplitPot().Skip(1);

        public Player CurrentPlayer => ActivePlayers
            .OrderBy(p => CurrentRound.LastPlay(p.Id))
            .ThenBy(p => p.Id)
            .First(p => CurrentRound.ShouldAct(p.Id));

        public static Game StartNew(BettingRules rules, Player[] players, Deck deck)
        {
            var activePlayers = players
                .Select(p => new Player(p.Id, deck.Take(2), true, p.Stack))
                .ToArray();

            var table = deck.Take(5);
            return StartNew(rules, activePlayers, table);
        }

        public static Game StartNew(BettingRules rules, Player[] players, Hand table)
        {
            if (players.Length < 2)
                throw new GameLogicException("Can't start round. Not enough players.");

            if (rules.BigBlind == 0)
                return new Game(rules, players.ToImmutableArray(), table, ImmutableArray.Create(Round.StartNew(players.Length)));

            var acts = new[]
            {
                new Act(0, Play.Blind, rules.SmallBlind),
                new Act(1, Play.Blind, rules.BigBlind)
            };

            var round = new Round(acts, players.Length);
            var rounds = ImmutableArray.Create(round);

            var blindPlayers = new[]
            {
                players[0].WithStack(s => s - rules.SmallBlind),
                players[1].WithStack(s => s - rules.BigBlind)
            };

            var playersList = blindPlayers.Concat(players.Skip(2)).ToImmutableArray();

            return new Game(rules, playersList, table, rounds);
        }

        public Game NextRound()
        {
            if (IsComplete)
                throw new GameLogicException("Can't start next round. Game is over.");

            if (!CurrentRound.IsComplete)
                throw new GameLogicException("Can't start next round. Current is incomplete.");

            var players = KeepActive(Players.ToArray(), CurrentRound.ActivePlayers);
            var next = Round.StartNew(players.Count(p => p.IsActive));
            var rounds = Rounds.Add(next);

            return new Game(Rules, players, Table, rounds);
        }

        private ImmutableArray<Player> KeepActive(IList<Player> players, IEnumerable<int> active)
        {
            var activePlayers = new HashSet<int>(active);
            var result = new Player[players.Count];

            var activeIndex = 0;
            for (var i = 0; i < players.Count; i++)
            {
                if (players[i].IsActive)
                {
                    if (!activePlayers.Contains(activeIndex++))
                    {
                        result[i] = players[i].MakeInactive();
                        continue;
                    }
                }

                result[i] = players[i];
            }

            return result.ToImmutableArray();
        }

        public Game MakeAct(Play play, int bet = 0)
        {
            return MakeAct(CurrentPlayer, play, bet);
        }

        public Game MakeAct(Player player, Play play, int bet = 0)
        {
            if (CurrentRound.IsComplete)
                throw new GameOrderException("Can not make act. Round is complete");

            if (player.Id != CurrentPlayer.Id)
                throw new GameOrderException($"Current player is {CurrentPlayer}");

            var options = GetOptions();
            if (!options.Contains(play))
                throw new GameLogicException($"Player is not supposed to make '{play}'");

            if (play == Play.Fold || play == Play.Check)
                bet = 0;

            var act = new Act(player.Id, play, bet);
            var rounds = Rounds.Replace(CurrentRound, CurrentRound.MakeAct(act));
            var players = Players.Replace(player, player.WithStack(s => s - bet));

            return new Game(Rules, players, Table, rounds);
        }

        public RoundState GetPlayerState(int id)
        {
            var player = Players.First(p => p.Id == id);
            if (!player.IsActive)
                return RoundState.Inactive;

            return new RoundState
            {
                Bet = CurrentRound.PlayerBet(id),
                IsActive = true,
                IsCurrent = id == CurrentPlayer.Id,
                LastPlay = CurrentRound.LastPlay(id)
            };
        }

        public Play[] GetOptions()
        {
            if (CurrentRound.IsComplete)
                return Array.Empty<Play>();

            var options = CurrentRound.GetOptions(CurrentPlayer.Id).ToImmutableArray();

            if (CurrentRound.MaxBet >= CurrentPlayer.Stack)
            {
                options = options
                    .Replace(Play.Call, Play.AllIn)
                    .Remove(Play.Raise);
            }

            return options.ToArray();
        }

        private IEnumerable<Player> TakeContributors(IDictionary<int, int> bets) => bets
            .Where(b => b.Value > 0)
            .Select(b => Players[b.Key]);

        private IEnumerable<Pot> SplitPot()
        {
            var acts = Rounds.SelectMany(r => r.Acts).ToArray();
            var playersBets = Players.ToDictionary(p => (int) p.Id, p => 0);

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
                    playersBets.Where(p => p.Value == maxBet).Select(p => Players[p.Key]),
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
                    if (playersBets[player.Id] >= contribution)
                    {
                        contenders.Add(player);
                        playersBets[player.Id] -= contribution;
                        amount += contribution;
                    }
                    else
                    {
                        amount += playersBets[player.Id];
                        playersBets[player.Id] = 0;
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

        public Player[] GetResult()
        {
            var pots = SplitPot();
            var result = Players.ToArray();

            foreach (var pot in pots)
            {
                if (pot.Contenders.Count() == 1)
                {
                    var winner = pot.Contenders.Single();
                    var index = Array.FindIndex(result, p => p.Id == winner.Id);
                    result[index] = winner.WithStack(s => s + pot);
                    continue;
                }

                var winners = GetWinners(Players)
                    .Select(p => p.Id)
                    .ToImmutableHashSet();

                var gain = pot / winners.Count;

                for (var i = 0; i < Players.Length; i++)
                {
                    if (winners.Contains(Players[i].Id))
                    {
                        result[i] = result[i].WithStack(s => s + gain);
                    }
                }
            }

            return result;
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
