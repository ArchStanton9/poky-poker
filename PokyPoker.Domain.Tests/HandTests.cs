﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PokyPoker.Domain.Tests.Utils;

namespace PokyPoker.Domain.Tests
{
    [TestFixture]
    public class HandTests
    {
        [TestCase("7S AS", HandName.HighCard)]
        [TestCase("7S 8S", HandName.HighCard)]
        [TestCase("7S 8H", HandName.HighCard)]

        [TestCase("7S 7H", HandName.OnePair)]
        [TestCase("7S 4D 7H", HandName.OnePair)]
        [TestCase("KH AS QD AH", HandName.OnePair)]
        [TestCase("KH AS QD AH 7D", HandName.OnePair)]

        [TestCase("7S 7H 5C 5D 2S", HandName.TwoPair)]
        [TestCase("7S 7H 5C 5H", HandName.TwoPair)]
        [TestCase("3S 7S 5C 7H 5H", HandName.TwoPair)]

        [TestCase("7S 7H 7C", HandName.ThreeOfAKind)]
        [TestCase("7S 7H 4D 2H 7D", HandName.ThreeOfAKind)]

        [TestCase("5S 5H 5C 5D AH", HandName.FourOfAKind)]
        [TestCase("5S 5H AH 5C 5D", HandName.FourOfAKind)]
        [TestCase("AH 5H 5C 5D 5S", HandName.FourOfAKind)]
        [TestCase("5H 5S 5C 5D", HandName.FourOfAKind)]

        [TestCase("5H 5S 5C 4D 4H", HandName.FullHouse)]
        [TestCase("AD AH 5S 5C 5D", HandName.FullHouse)]

        [TestCase("2D 3H 4S 5C 6D", HandName.Straight)]
        [TestCase("2D 3H 4S 5C AD", HandName.Straight)]

        [TestCase("JD AD 8D 10D 2D", HandName.Flush)]

        [TestCase("2D 3D 4D 5D 6D", HandName.StraightFlush)]
        public void Can_find_hand_name(string handString, HandName name)
        {
            var cards = CardsParser.ParseMany(handString);
            var hand = new Hand(cards);
            hand.Name.Should().Be(name);
        }

        [Test]
        public void PerfTest()
        {
            var hands = new[]
                {
                    "5S 5H AH 5C 5D",
                    "2D 3D 4D 5D 6D",
                    "JD AD 8D 10D 2D",
                    "KH AS QD AH",
                    "3S 7S 5C 7H 5H"
                }.Select(CardsParser.ParseMany)
                .ToArray();


            var watch = Stopwatch.StartNew();

            for (var i = 0; i < 100000; i++)
            {
                foreach (var hand in hands)
                {
                    Hand.GetHandName(hand);
                }
            }

            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }


        [Test]
        public void Can_combine_FullHouse_on_river()
        {
            var hand = CardsParser.ParseHand("QH AS");
            var table = CardsParser.ParseMany("QD 8C AC AD 7S");

            using (ConsoleStopWatch.StartNew())
            {
                hand = hand.Combine(table);
            }

            hand.Name.Should().Be(HandName.FullHouse);
        }

        [Test]
        public void Can_combine_FourOfKind_on_turn()
        {
            var hand = CardsParser.ParseHand("QH AS");
            var table = CardsParser.ParseMany("AH 8C AC AD 7S");

            using (ConsoleStopWatch.StartNew())
            {
                hand = hand.Combine(table);
            }

            hand.Name.Should().Be(HandName.FourOfAKind);
        }

        [TestCase("QH QS 7D 8D 2H", "QC QD 7S 8S 2D", 0)]
        [TestCase("QH QS", "AC AD", -1)]
        [TestCase("KH KS", "JC JD", 1)]
        [TestCase("7H 7S 7D", "7C 7S 7D", 0)]
        [TestCase("7H 7S 7D KH", "7C JS 7D 2S", 1)]
        [TestCase("4H 6H AH JH 2H", "5C 6S 7D 4S 3D", 1)]
        [TestCase("AH KC QS JH 10H", "AC 2S 3D 4S 5D", 1)]
        public void Compare(string h1, string h2, int expected)
        {
            var hand1 = CardsParser.ParseHand(h1);
            var hand2 = CardsParser.ParseHand(h2);

            hand1.CompareTo(hand2).Should().Be(expected);
        }
    }
}
