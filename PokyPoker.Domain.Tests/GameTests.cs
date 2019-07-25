using FluentAssertions;
using NUnit.Framework;
using PokyPoker.Domain.Tests.Utils;

namespace PokyPoker.Domain.Tests
{
    [TestFixture]
    class GameTests
    {
        private BettingRules rules;
        private Deck deck;
        private Game game;
        private Player p1;
        private Player p2;
        private Player p3;

        [SetUp]
        public void SetUp()
        {
            deck = Deck.BuildStandard();

            p1 = new Player(0, deck.Take(2), true, 5000);
            p2 = new Player(1, deck.Take(2), true, 4000);
            p3 = new Player(2, deck.Take(2), true, 6000);

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
            game.CurrentPlayer.Spot.Should().Be(p3.Spot);
        }

        [Test]
        public void Test2()
        {
            game.MakeAct(Play.Call, 40)
                .MakeAct(Play.Fold)
                .MakeAct(Play.Check)
                .NextRound()
                .MakeAct(Play.Check)
                .MakeAct(Play.Check);
        }

        [Test]
        public void Small_blind_player_should_be_able_to_raise()
        {
            game.MakeAct(Play.Call, 40)
                .GetOptions()
                .Should().Contain(Play.Raise);
        }

        [Test]
        public void Can_make_all_in_if_stack_to_low()
        {
            var options = GamesBuilder
                .Create(400, 500, 20)
                .GetOptions();

            options.Should().Contain(Play.AllIn);
        }

        [Test]
        public void PreFlop_winner_test()
        {
            game = GamesBuilder.Create(rules, 100, 100, 100)
                .MakeAct(Play.Raise, 100)
                .MakeAct(Play.Fold)
                .MakeAct(Play.Fold);

            var result = game.GetResult();
            result[2].Stack.Should().Be(160);
        }

        [Test]
        public void Missing_seats_PreFlop_test()
        {
            var players = new[]
            {
                new Player(0, deck.Take(2), true, 5000),
                new Player(4, deck.Take(2), true, 5000),
                new Player(6, deck.Take(2), true, 5000),
            };

            game = Game.StartNew(rules, players, deck.Take(5));
            game.CurrentPlayer.Spot.Should().Be(6);
        }

        [Test]
        public void Missing_seats_Flop_test()
        {
            var players = new[]
            {
                new Player(0, deck.Take(2), true, 5000),
                new Player(4, deck.Take(2), true, 5000),
                new Player(6, deck.Take(2), true, 5000),
            };

            game = Game.StartNew(rules, players, deck.Take(5))
                .MakeAct(Play.Fold)
                .MakeAct(Play.Call, 20)
                .MakeAct(Play.Check)
                .NextRound();

            game.CurrentPlayer.Spot.Should().Be(0);
        }
    }
}
