using FluentAssertions;
using NUnit.Framework;
using OfflinePoker.Domain.Exceptions;

namespace OfflinePoker.Domain.Tests
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
                .MakeAct(Play.Call, 20)
                .MakeAct(Play.Check);

            initialRound.IsComplete.Should().BeTrue();
        }

        [Test]
        public void Can_call_for_big_blind()
        {
            initialRound.Invoking(r => r.MakeAct(Play.Call, 20))
                .Should().NotThrow<GameException>();
        }

        [Test]
        public void Can_Complete_round_with_3_players()
        {
            var round = Round.StartNew(3)
                .MakeAct(Play.Bet, 5)
                .MakeAct(Play.Fold)
                .MakeAct(Play.Call, 5);

            round.IsComplete.Should().BeTrue();
        }

    }
}
