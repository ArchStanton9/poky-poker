using System.Diagnostics;

namespace PokyPoker.Domain.Tests.Utils
{
    public static class RoundExtensions
    {
        [DebuggerStepThrough]
        public static Round MakeAct(this Round round, Player player, Play play, int bet = 0) =>
            round.MakeAct(new Act(player, play, bet));

    }
}
