using FluentAssertions;
using NUnit.Framework;
using PokyPoker.Domain.Exceptions;
using PokyPoker.Domain.Tests.Utils;

namespace PokyPoker.Domain.Tests
{
    public class RoundTests
    {
        private Round initialRound;

        private Player player1;
        private Player player2;



        [SetUp]
        public void SetUp()
        {
            var deck = Deck.BuildStandard();
            player1 = new Player(0, deck.Take(2), true, 5000);
            player2 = new Player(1, deck.Take(2), true, 10000);

            initialRound = new Round(new[]
            {
                new Act(player1, Play.Blind, 20),
                new Act(player2, Play.Blind, 40),
            }, new[] {player1, player2});
        }

        [Test]
        public void Can_complete()
        {
            initialRound = initialRound
                .MakeAct(player1, Play.Call, 20)
                .MakeAct(player2, Play.Check);

            initialRound.IsComplete.Should().BeTrue();
        }

        [Test]
        public void Can_call_for_big_blind()
        {
            initialRound.Invoking(r => r.MakeAct(player1, Play.Call, 20))
                .Should().NotThrow<GameException>();
        }
    }
}
