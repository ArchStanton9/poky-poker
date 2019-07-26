using System.Linq;
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
            var game = GamesBuilder.CreateNoBlinds(500, 40, 450)
                .MakeAct(Play.Bet, 150)
                .MakeAct(Play.AllIn, 40)
                .MakeAct(Play.Call, 150);

            game.MainPot.Amount.Should().Be(120);
            game.SidePots.Should().ContainSingle().Which.Amount.Should().Be(340 - 120);
        }

        [Test]
        public void Second_AllIns()
        {
            var game = GamesBuilder.CreateNoBlinds(500, 40, 50, 450)
                .MakeAct(Play.Bet, 150)
                .MakeAct(Play.AllIn, 40)
                .MakeAct(Play.AllIn, 50)
                .MakeAct(Play.Call, 150);

            game.MainPot.Amount.Should().Be(160);
            game.SidePots.Select(p => (int) p).Should().ContainInOrder(30, 390 - 160 - 30);
        }

        [Test]
        public void SubPotsOnFlop()
        {
            var game = GamesBuilder.Create(500, 200, 660)
                .MakeAct(Play.Raise, 100)
                .MakeAct(Play.Call, 80)
                .MakeAct(Play.Call, 60)
                // NextRound
                .MakeAct(Play.Bet, 100)
                .MakeAct(Play.AllIn, 100)
                .MakeAct(Play.Call, 100)
                // NextRound
                .MakeAct(Play.Bet, 50)
                .MakeAct(Play.Raise, 100);

            game.MainPot.Amount.Should().Be(600);
            game.SidePots.Should().ContainSingle().Which.Amount.Should().Be(150);
        }
    }
}
