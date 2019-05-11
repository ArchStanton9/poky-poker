using FluentAssertions;
using NUnit.Framework;
using PokyPoker.Domain.Exceptions;
using PokyPoker.Domain.Tests.Utils;

namespace PokyPoker.Domain.Tests
{
    public class RoundTests
    {
        private Round initialRound;

        [SetUp]
        public void SetUp()
        {
            initialRound = new Round(new[]
            {
                new Act(0, Play.Blind, 20),
                new Act(1, Play.Blind, 40),
            }, 2);
        }

        [Test]
        public void Can_complete()
        {
            initialRound = initialRound
                .MakeAct(0, Play.Call, 20)
                .MakeAct(1, Play.Check);

            initialRound.IsComplete.Should().BeTrue();
        }

        [Test]
        public void Can_call_for_big_blind()
        {
            initialRound.Invoking(r => r.MakeAct(0, Play.Call, 20))
                .Should().NotThrow<GameException>();
        }

        [Test]
        public void Can_Complete_round_with_3_players()
        {
            var round = Round.StartNew(3)
                .MakeAct(0, Play.Bet, 5)
                .MakeAct(1, Play.Fold)
                .MakeAct(2, Play.Call, 5);

            round.IsComplete.Should().BeTrue();
        }
    }
}
