using System.Linq;
using PokyPoker.Domain;

namespace PokyPoker.Desktop
{
    public static class HandFormatter
    {
        public static string Format(Hand hand)
        {
            var cards = hand.Select(CardsParser.ToString);
            return string.Join(" ", cards);
        }
    }
}
