using FluentAssertions;
using NUnit.Framework;

namespace PokyPoker.Domain.Tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void Can_parse_queen_of_hearts()
        {
            var card = CardsParser.Parse("QH");
            card.Rank.Should().Be(Rank.Queen);
            card.Suit.Should().Be(Suit.Hearts);
        }

        [Test]
        public void Can_parse_cards_set()
        {
            var cards = CardsParser.ParseMany("QH KD 10S");
            cards.Should().HaveCount(3);
        }
    }
}
