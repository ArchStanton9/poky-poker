using System.Linq;

namespace PokyPoker.Domain.Tests.Utils
{
    public static class GamesBuilder
    {
        public static Game Create(params int[] stacks) => Create(BettingRules.Standard, stacks);

        public static Game CreateNoBlinds(params int[] stacks) => Create(BettingRules.NoBlinds, stacks);

        public static Game Create(BettingRules rules, params int[] stacks)
        {
            var deck = Deck.BuildStandard();

            byte id = 0;
            var players = stacks
                .Select(s => new Player(id++, deck.Take(2), true, s))
                .ToArray();

            return Game.StartNew(rules, players, deck);
        }
    }
}
