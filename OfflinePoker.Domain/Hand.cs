using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Name = GetHandName(cards);
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

        #region Helpers

        public static HandName GetHandName(Card[] cards)
        {
            var (ranks, suits) = Decompose(cards);
            var flash = IsFlush(suits);
            var straight = IsStraight(ranks);

            if (flash && straight) return HandName.StraightFlush;
            if (flash) return HandName.Flush;
            if (straight) return HandName.Straight;

            return CheckName(new ArraySegment<byte>(ranks));
        }

        private static (byte[], byte[]) Decompose(Card[] cards)
        {
            var ranks = cards
                .Select(c => (byte) c.Rank)
                .OrderByDescending(x => x)
                .ToArray();

            var suits = cards
                .Select(c => (byte) c.Suit)
                .ToArray();

            return (ranks, suits);
        }

        public static bool IsFlush(byte[] suits)
        {
            var current = suits[0];
            for (int i = 1; i < suits.Length; i++)
            {
                if (current != suits[i])
                    return false;
            }

            return true;
        }


        public static bool IsStraight(byte[] ranks)
        {

#if DEBUG
            AssertSorted(ranks);
#endif
            
            var current = ranks[0];
            for (var i = 1; i < ranks.Length; i++)
            {
                if (ranks[i] - current != 1)
                    return false;

                current = ranks[i];
            }

            return true;
        }

        private static int MaxRepeats(byte[] array, out byte upper)
        {

#if DEBUG
            AssertSorted(array);
#endif

            var count = 1;
            upper = array[0];
            for (var i = 1; i < array.Length; i++)
            {
                if (upper == array[i])
                {
                    count++;
                }
                else
                {
                    if (count > 1)
                        return count;

                    upper = array[i];
                    count = 1;
                }
            }

            return count;
        }

        public static HandName CheckName(ArraySegment<byte> ranks)
        {
#if DEBUG
            AssertSorted(ranks.Array);
#endif

            var layout = ParseLayout(ranks);
            return layoutsMap.TryGetValue(layout, out var name) ? name : HandName.HighCard;
        }

        private static readonly Dictionary<int, HandName> layoutsMap = new Dictionary<int, HandName>
        {
            {2, HandName.OnePair},
            {3, HandName.ThreeOfAKind},
            {4, HandName.FourOfAKind},
            {22, HandName.TwoPair},
            {32, HandName.FullHouse},
            {23, HandName.FullHouse}
        };


        public static int ParseLayout(ArraySegment<byte> segment)
        {
            var result = 0;

            for (var i = 0; i < 5; i++)
            {
                if (segment.Count == 0)
                    return result;

                segment = Take(segment, out var c);
                if (c < 2)
                    continue;

                result = result == 0 ? c : result * 10 + c;
            }

            return result;
        }

        public static ArraySegment<byte> Take(ArraySegment<byte> segment, out int count)
        {
            Debug.Assert(segment.Array != null, "segment.Array != null");

            var edge = NextEdge(segment);
            if (edge < 0)
            {
                count = 0;
                return new ArraySegment<byte>(segment.Array, segment.Array.Length - 1, 0);
            }

            count = edge - segment.Offset;
            var start = segment.Offset + count;
            var end = segment.Array.Length - start;

            return new ArraySegment<byte>(segment.Array, start, end);
        }

        public static int NextEdge(ArraySegment<byte> segment)
        {
            var array = segment.Array;
            if (array == null)
                return 0;

            var i = segment.Offset;
            var current = array[i++];
            for (; i < segment.Offset + segment.Count; i++)
            {
                if (array[i] != current)
                    return i;
            }

            return i;
        }

        private static void AssertSorted(byte[] array)
        {
            if (!IsSorted(array))
                throw new ArgumentException("Array is supposed to be sorted.", nameof(array));
        }
        

        private static bool IsSorted(byte[] array)
        {
            if (array.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(array));

            var current = array[0];
            for (var i = 1; i < array.Length; i++)
            {
                if (current < array[i])
                    return false;

                current = array[i];
            }

            return true;
        }

        #endregion
    }
}
