using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace OfflinePoker.Domain.Tests
{
    [TestFixture]
    public class HandTests
    {
        [Test]
        public void Foo()
        {
            Console.WriteLine(sizeof(short));


            var hand = new Hand(
                new Card(Rank.Seven, Suit.Hearts),
                new Card(Rank.Seven, Suit.Clubs),
                new Card(Rank.Ten, Suit.Diamonds)
            );

        }

        [Test]
        public void Pair_test()
        {
            var hand = new Hand(
                new Card(Rank.Seven, Suit.Hearts),
                new Card(Rank.Seven, Suit.Clubs),
                new Card(Rank.Ten, Suit.Diamonds)
            );

            // 7, 0
            // 7, 3
            // 10, 1

            hand.Name.Should().Be(HandName.OnePair);
        }

        [Test]
        public void Next_egde_test()
        {
            var array = new byte[] {5, 4, 4, 2, 1};
            var l = Hand.ParseLayout(array);
        }
    }
}
