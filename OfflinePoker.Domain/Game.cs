using System.Linq;
using OfflinePoker.Domain.Exceptions;

namespace OfflinePoker.Domain
{
    public class Game
    {
        public Game(Player[] players, Hand table, Round[] rounds)
        {
            Players = players;
            Table = table;
            Rounds = rounds;
        }

        public Player[] Players { get; }
        public Hand Table { get; }
        public Round[] Rounds { get; }

        public Stage Stage => (Stage) Rounds.Count();
        public Round CurrentRound => Rounds[Rounds.Length - 1];
        public bool IsComplete => Stage == Stage.River && CurrentRound.IsComplete;
        public Game NextRound() => NextRound(this);

        public static Game NextRound(Game game)
        {
            if (game.IsComplete)
                throw new GameLogicException("Can't start next round. Game is over.");

            if (!game.CurrentRound.IsComplete)
                throw new GameLogicException("Can't start next round. Current is incomplete.");

            var roundsCount = game.Rounds.Length;
            var next = game.CurrentRound.StartNext();
            var rounds = roundsCount == 0
                ? new[] {next}
                : game.Rounds
                    .Append(next)
                    .ToArray();

            return new Game(game.Players, game.Table, rounds);
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
