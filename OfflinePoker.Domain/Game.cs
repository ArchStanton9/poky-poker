using System.Collections.Generic;
using System.Linq;
using OfflinePoker.Domain.Exceptions;

namespace OfflinePoker.Domain
{
    public class Game
    {
        public Game(BettingRules rules, Player[] players, Hand table, Round[] rounds)
        {
            Rules = rules;
            Players = players;
            Table = table;
            Rounds = rounds;
            ActivePlayers = Players.Where(p => p.IsActive).ToArray();
        }

        public BettingRules Rules { get; }
        public IReadOnlyList<Player> Players { get; }
        public Hand Table { get; }
        public IReadOnlyList<Round> Rounds { get; }
        public IReadOnlyList<Player> ActivePlayers { get; }
        public Stage Stage => (Stage) Rounds.Count;
        public Round CurrentRound => Rounds.Last();
        public bool IsComplete => Stage == Stage.River && CurrentRound.IsComplete;
        public Player CurrentPlayer => ActivePlayers[CurrentRound.InTurn];

        public static Game StartNew(BettingRules rules, Player[] players, Hand table)
        {
            var round = Round.CreateInitial(rules, players.Length);
            return new Game(rules, players, table, new[] {round});
        }


        public Game NextRound()
        {
            if (IsComplete)
                throw new GameLogicException("Can't start next round. Game is over.");

            if (!CurrentRound.IsComplete)
                throw new GameLogicException("Can't start next round. Current is incomplete.");

            var players = KeepActive(Players.ToArray(), CurrentRound.ActivePlayers);
            var next = Round.StartNew(players.Count(p => p.IsActive));
            var rounds = Rounds.Append(next).ToArray();

            return new Game(Rules, players, Table, rounds);
        }

        public Player[] KeepActive(Player[] players, IEnumerable<int> active)
        {
            var activePlayers = new HashSet<int>(active);

            var activeIndex = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (!players[i].IsActive)
                    continue;

                if (!activePlayers.Contains(activeIndex))
                {
                    players[i] = players[i].Deactivate();
                }

                activeIndex++;
            }

            return players;
        }

        public Game MakeAct(Play play, int bet = 0)
        {
            return MakeAct(CurrentPlayer, play, bet);
        }

        public Game MakeAct(Player player, Play play, int bet = 0)
        {
            if (CurrentRound.IsComplete)
                throw new GameOrderException("Can not make act. Round is complete");

            if (player != CurrentPlayer)
                throw new GameOrderException($"Current player is {CurrentPlayer}");

            var round = CurrentRound.MakeAct(play, bet);
            var copy = Rounds.ToArray();
            copy[copy.Length - 1] = round;

            return new Game(Rules, Players.ToArray(), Table, copy);
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
