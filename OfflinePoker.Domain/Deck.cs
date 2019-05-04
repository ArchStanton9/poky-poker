using System;
using System.Collections.Generic;
using System.Linq;
using OfflinePoker.Domain.Helpers;

namespace OfflinePoker.Domain
{
    public class Deck
    {
        private readonly List<Card> cards;

        public Deck(IEnumerable<Card> cards)
        {
            this.cards = cards.ToList();
        }

        public IEnumerable<Card> Cards => cards;

        #region StaticHelpers

        public static Deck BuildStandard()
        {
            var cards = new List<Card>(52);
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                foreach (Suit suit in Enum.GetValues(typeof(Suit)))
                {
                    cards.Add(new Card(rank, suit));
                }
            }

            return new Deck(cards.Shuffle());
        }

        #endregion

        public Card Take()
        {
            var card = cards.First();
            cards.RemoveAt(0);
            return card;
        }

        public Hand Take(int count) => new Hand(
            Enumerable.Range(0, count)
                .Select(i => Take())
                .ToArray()
        );
    }
}
