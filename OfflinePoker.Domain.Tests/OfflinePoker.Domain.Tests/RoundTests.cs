using System;
using FluentAssertions;
using NUnit.Framework;
using OfflinePoker.Domain.Exceptions;

namespace OfflinePoker.Domain.Tests
{
    public class RoundTests
    {
        private Round round;
        private BettingRules rules;

        [SetUp]
        public void SetUp()
        {
            var h1 = new Hand(
                new Card(Rank.Ace, Suit.Spades),
                new Card(Rank.Ace, Suit.Clubs));

            var h2 = new Hand(
                new Card(Rank.King, Suit.Hearts),
                new Card(Rank.Queen, Suit.Hearts));

            var p1 = new Player("p1", h1, 0, 5000);
            var p2 = new Player("p2", h2, 1, 4000);


            rules = new BettingRules
            {
                BigBlind = 40,
                SmallBlind = 20
            };

            round = Round.CreateInitial(new[] { p1, p2 }, rules);
        }

        [Test]
        public void Can_complete()
        {
            round.MakeAct(Act.Call, 20);
            round.MakeAct(Act.Check);
            round.IsComplete.Should().BeTrue();
        }

        [Test]
        public void Can_call_for_big_blind()
        {
            round.Invoking(r => r.MakeAct(Act.Call, 20))
                .Should().NotThrow<GameException>();
        }
    }
}
