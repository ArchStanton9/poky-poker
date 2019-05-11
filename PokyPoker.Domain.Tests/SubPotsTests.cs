using FluentAssertions;
using NUnit.Framework;
using PokyPoker.Domain.Tests.Utils;

namespace PokyPoker.Domain.Tests
{
    [TestFixture]
    public class SubPotsTests
    {
        [Test]
        public void Single_AllIn()
        {
            var round = Round.StartNew(3)
                .MakeAct(0, Play.Bet, 150)
                .MakeAct(1, Play.AllIn, 40)
                .MakeAct(2, Play.Call, 150);

            round.SubPots().Should().ContainInOrder(120, 340 - 120);
        }

        [Test]
        public void Second_AllIns()
        {
            var round = Round.StartNew(4)
                .MakeAct(0, Play.Bet, 150)
                .MakeAct(1, Play.AllIn, 40)
                .MakeAct(2, Play.AllIn, 50)
                .MakeAct(3, Play.Call, 150);

            round.SubPots().Should().ContainInOrder(160, 30, 390 - 160 - 30);
        }

        [Test]
        public void SubPotsOnFlop()
        {
            var game = GamesBuilder.CreateNew(500, 200, 660)
                .MakeAct(Play.Raise, 100)
                .MakeAct(Play.Call, 80)
                .MakeAct(Play.Call, 60)
                .NextRound()
                .MakeAct(Play.Bet, 100)
                .MakeAct(Play.AllIn, 100)
                .MakeAct(Play.Call, 100)
                .NextRound()
                .MakeAct(Play.Bet, 50)
                .MakeAct(Play.Raise, 100);

            game.SubPots.Should().ContainInOrder(600, 150);
        }
    }
}
