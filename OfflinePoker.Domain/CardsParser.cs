using System;
using System.Collections.Generic;
using System.Linq;

namespace OfflinePoker.Domain
{
    public static class CardsParser
    {
        public static Card Parse(string text)
        {
            if (text.Length != 2 && text.Length != 3)
                throw new FormatException("Expected to string of length 2");

            text = text.ToUpper();
            var suitKey = text[text.Length - 1];
            var rankKey = text.Substring(0, text.Length - 1);

            if (ranksMap.TryGetValue(rankKey, out var rank) &&
                suitMap.TryGetValue(suitKey, out var suit))
                return new Card(rank, suit);

            throw new FormatException("Unexpected card format");
        }

        private static readonly Dictionary<char, Suit> suitMap = new Dictionary<char, Suit>()
        {
            {'H', Suit.Hearts},
            {'D', Suit.Diamonds},
            {'C', Suit.Clubs},
            {'S', Suit.Spades}
        };

        private static readonly Dictionary<string, Rank> ranksMap = BaseRanksMap
            .Concat(SpecialRanksMap)
            .ToDictionary(x => x.Key, x => x.Value);

        private static Dictionary<string, Rank> BaseRanksMap => Enumerable.Range(2, 10)
            .ToDictionary(i => i.ToString(), i => (Rank) i);

        private static Dictionary<string, Rank> SpecialRanksMap => new Dictionary<string, Rank>
        {
            {"A", Rank.Ace},
            {"J", Rank.Jack},
            {"Q", Rank.Queen},
            {"K", Rank.King},
        };


        public static Card[] ParseMany(string text)
        {
            var chunks = text.Split(' ');
            return chunks.Select(Parse).ToArray();
        }

        public static Hand ParseHand(string text)
        {
            return new Hand(ParseMany(text));
        }
    }
}
