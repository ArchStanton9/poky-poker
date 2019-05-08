using System.Collections.Generic;
using System.Linq;
using PokyPoker.Domain;

namespace PokyPoker.Desktop
{
    public static class HandFormatter
    {
        public static string Format(IEnumerable<Card> hand)
        {
            var cards = hand.Select(CardsParser.ToString);
            return string.Join(" ", cards);
        }
    }
}
