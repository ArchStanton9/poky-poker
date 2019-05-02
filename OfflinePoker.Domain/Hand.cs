using System;
using System.Linq;

namespace OfflinePoker.Domain
{
    public class Hand
    {
        private readonly Card[] cards;

        public Hand(params Card[] cards)
        {
            if (cards.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(cards));

            this.cards = cards;
            Name = GetName(cards);
        }

        private static HandName GetName(Card[] cards)
        {
            return HandName.HighCard;
        }

        public Hand Add(Card card)
        {
            return new Hand(cards.Append(card).ToArray());
        }

        public HandName Name { get; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}
