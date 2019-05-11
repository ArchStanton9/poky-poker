using System.Diagnostics;

namespace PokyPoker.Domain.Tests.Utils
{
    public static class RoundExtensions
    {
        [DebuggerStepThrough]
        public static Round MakeAct(this Round round, byte playerId, Play play, int bet = 0) =>
            round.MakeAct(new Act(playerId, play, bet));

    }
}
