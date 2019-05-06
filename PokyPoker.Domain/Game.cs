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
            ActivePlayers = Players.Where(p => p.IsActive).ToArray();
        }

        public BettingRules Rules { get; }
        public ImmutableArray<Player> Players { get; }
        public Hand Table { get; }
        public ImmutableArray<Round> Rounds { get; }
        public IList<Player> ActivePlayers { get; }
        public Stage Stage => (Stage) Rounds.Length;
        public Round CurrentRound => Rounds.Last();
        public bool IsComplete => Stage == Stage.River && CurrentRound.IsComplete;

        public Player CurrentPlayer => ActivePlayers[CurrentRound.InTurn];
        public int Pot => Rounds.Aggregate(0, (p, r) => p + r.Pot);
        
        public static Game StartNew(BettingRules rules, Player[] players, Deck deck)
        {
            var activePlayers = players
                .Select(p => new Player(p.Name, deck.Take(2), true, p.Stack))
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

            if (player.Name != CurrentPlayer.Name)
                throw new GameOrderException($"Current player is {CurrentPlayer}");

            if (play == Play.Fold || play == Play.Check)
                bet = 0;

            var rounds = Rounds.Replace(CurrentRound, CurrentRound.MakeAct(play, bet));
            var players = Players.Replace(player, player.WithStack(s => s - bet));

            return new Game(Rules, players, Table, rounds);
        }

        public RoundState GetPlayerState(string name)
        {
            var player = Players.First(p => p.Name == name);
            if (!player.IsActive)
                return RoundState.Inactive;

            var index = ActivePlayers.IndexOf(player);
            return CurrentRound.GetPlayerState(index);
        }

        public Play[] GetOptions()
        {
            if (CurrentRound.IsComplete)
                return Array.Empty<Play>();

            return CurrentRound.GetOptions(CurrentRound.InTurn);
        }

        public Player[] GetResult()
        {
            if (Stage == Stage.River && CurrentRound.IsComplete && !CurrentRound.HasWinner)
            {

            }

            if (CurrentRound.HasWinner)
            {
                var winnerIndex = CurrentRound.ActivePlayers.Single();
                var winner = ActivePlayers.ElementAt(winnerIndex);

                return Players.Replace(winner, winner.WithStack(s => s + Pot)).ToArray();
            }

            throw new NotImplementedException();
        }
    }

    public enum Stage
    {
        PreFlop,
        Flop,
        Turn,
        River
    }
}
