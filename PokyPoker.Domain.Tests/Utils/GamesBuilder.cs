using System.Linq;

namespace PokyPoker.Domain.Tests.Utils
{
    public static class GamesBuilder
    {
        public static Game CreateNew(params int[] stacks)
        {
            var deck = Deck.BuildStandard();

            byte id = 0;
            var players = stacks
                .Select(s => new Player(id++, deck.Take(2), true, s))
                .ToArray();

            return Game.StartNew(BettingRules.Standard, players, deck);
        }
    }
}
