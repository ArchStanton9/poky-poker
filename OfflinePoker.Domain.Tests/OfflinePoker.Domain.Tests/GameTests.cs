using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace OfflinePoker.Domain.Tests
{
    [TestFixture]
    class GameTests
    {
        private BettingRules rules;
        private Game game;
        private Player p1;
        private Player p2;
        private Player p3;

        [SetUp]
        public void SetUp()
        {
            var deck = Deck.CreateStandardDeck();

            p1 = new Player("p1", deck.Take(2), true, 5000);
            p2 = new Player("p2", deck.Take(2), true, 4000);
            p3 = new Player("p3", deck.Take(2), true, 6000);

            rules = new BettingRules
            {
                BigBlind = 40,
                SmallBlind = 20
            };

            var players = new[] {p1, p2, p3};
            var table = deck.Take(5);

            game = Game.StartNew(rules, players, table);
        }

        [Test]
        public void Left_for_big_blind_should_act_first()
        {
            game.CurrentPlayer.Name.Should().Be(p3.Name);
        }

        [Test]
        public void Test2()
        {
            game.MakeAct(Play.Call, 40)
                .MakeAct(Play.Check)
                .MakeAct(Play.Fold)
                .NextRound()
                .MakeAct(Play.Check)
                .MakeAct(Play.Check);
        }
    }
}
