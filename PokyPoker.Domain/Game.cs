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
        public bool IsComplete => Stage == Stage.River && CurrentRound.IsComplete;
        public int Pot => Rounds.Aggregate(0, (p, r) => p + r.Pot);
        public int[] SubPots => GetSubPots(Rounds);

        public Player CurrentPlayer => ActivePlayers
            .OrderBy(p => CurrentRound.LastPlay(p.Id))
            .ThenBy(p => p.Id)
            .First(p => CurrentRound.ShouldAct(p.Id));

        private static int[] GetSubPots(ImmutableArray<Round> rounds)
        {
            var initial = rounds[0].HasSubPots ? rounds[0].SubPots() : new[] {rounds[0].Pot};
            var stack = new Stack<int>(initial);

            for (var i = 1; i < rounds.Length; i++)
            {
                var round = rounds[i];
                if (round.HasSubPots)
                {
                    var subPots = round.SubPots();
                    stack.Push(subPots[0] + stack.Pop());
                    for (var j = 1; j < subPots.Length; j++)
                    {
                        stack.Push(subPots[i]);
                    }

                    if (subPots.Length == 1)
                        stack.Push(0);
                }
                else
                {
                    stack.Push(stack.Pop() + round.Pot);
                }
            }

            return stack.Reverse().ToArray();
        }

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

        public ImmutableArray<Player> KeepActive(IList<Player> players, IEnumerable<int> active)
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

        public RoundState GetPlayerState(byte id)
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

        public Player[] GetWinners(IList<Player> players)
        {
            var winners = new List<Player>();
            foreach (var player in players.Select(p => p.WithHand(h => h.Combine(Table))))
            {
                if (winners.Count == 0)
                {
                    winners.Add(player);
                    continue;
                }

                if (player.Hand > winners[0].Hand)
                {
                    winners.Clear();
                }
                else if (player.Hand < winners[0].Hand)
                {
                    continue;
                }

                winners.Add(player);
            }

            return Players
                .Where(p => winners.Any(w => w.Id == p.Id))
                .ToArray();
        }


        public Player[] GetResult()
        {
            if (Stage == Stage.River && CurrentRound.IsComplete && !CurrentRound.HasWinner)
            {
                var winners = GetWinners(ActivePlayers);
                var gain = Pot / winners.Length;
                return winners
                    .Aggregate(Players, (c, w) => c.Replace(w, w.WithStack(s => s + gain)))
                    .ToArray();
            }

            if (CurrentRound.HasWinner)
            {
                var winnerIndex = CurrentRound.ActivePlayers.Single();
                var winner = ActivePlayers.ElementAt(winnerIndex);

                return Players.Replace(winner, winner.WithStack(s => s + Pot)).ToArray();
            }

            throw new NotImplementedException();
        }

        public Card[] GetCurrentTable()
        {
            switch (Stage)
            {
                case Stage.PreFlop:
                    return new Card[0];
                case Stage.Flop:
                    return Table.Take(3).ToArray();
                case Stage.Turn:
                    return Table.Take(4).ToArray();
                case Stage.River:
                    return Table.Take(5).ToArray();
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
