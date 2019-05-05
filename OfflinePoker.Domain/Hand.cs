using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OfflinePoker.Domain.Exceptions;

namespace OfflinePoker.Domain
{
    public class Hand : IEnumerable<Card>
    {
        private readonly Card[] cards;
        private readonly Lazy<HandName> name;

        public Hand(params Card[] cards)
        {
            if (cards.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(cards));

            this.cards = cards;
            name = new Lazy<HandName>(() => GetHandName(cards));
        }

        public Hand Add(Card card)
        {
            return new Hand(cards.Append(card).ToArray());
        }

        public HandName Name => name.Value;

        public override string ToString()
        {
            return Name.ToString();
        }

        #region Compare

        private struct Swap
        {
            public Swap(int a, int b)
            {
                A = a;
                B = b;
            }

            public readonly int A;
            public readonly int B;

            public override string ToString()
            {
                return $"{A} -> {B}";
            }
        }

        private class TemporarySwap : IDisposable
        {
            private readonly Card[] table;
            private readonly Card[] hand;
            private readonly Swap[] swaps;

            public TemporarySwap(Card[] table, Card[] hand, params Swap[] swaps)
            {
                this.table = table;
                this.hand = hand;
                this.swaps = swaps;

                foreach (var swap in swaps)
                {
                    var temp = table[swap.B];
                    table[swap.B] = hand[swap.A];
                    hand[swap.A] = temp;
                }
            }

            public void Dispose()
            {
                foreach (var swap in swaps)
                {
                    var temp = hand[swap.A];
                    hand[swap.A] = table[swap.B];
                    table[swap.B] = temp;
                }
            }
        }

        public Hand Combine(Hand table) => Combine(table.cards);

        public Hand Combine(Card[] table)
        {
            if (cards.Length != 2)
                throw new GameLogicException("Can combine only players hand of two cards");

            switch (table.Length)
            {
                case 5: return CombineRiver(table);
                case 4: return CombineTurn(table);
                case 3: return new Hand(table[0], table[1], table[3], cards[0], cards[1]);
                default:
                    throw new GameLogicException("Can combine hand only with table cards");
            }
        }

        private static readonly Lazy<IEnumerable<Swap[]>> riverSwaps
            = new Lazy<IEnumerable<Swap[]>>(GetRiverSwaps);

        private static List<Swap[]> GetRiverSwaps()
        {
            var table = new[] {0, 1, 2, 3, 4};
            var hand = new[] {0, 1};

            var swaps = table
                .SelectMany(x => hand.Select(y => new Swap(y, x)))
                .ToArray();

            var options = new List<Swap[]>();

            for (var i = 0; i < swaps.Length; i++)
            {
                for (var j = i + 1; j < swaps.Length; j++)
                {
                    var p1 = swaps[i];
                    var p2 = swaps[j];

                    if (p1.A != p2.A && p1.B != p2.B)
                        options.Add(new[] {swaps[i], swaps[j]});
                }
            }

            options.AddRange(swaps.Select(x => new[] {x}));

            return options;
        }

        private Hand CombineRiver(Card[] table)
        {
            var bestHand = new Hand(table);
            var bestHandName = bestHand.Name;
            
            foreach (var swap in riverSwaps.Value)
            {
                using (new TemporarySwap(table, cards, swap))
                {
                    var handName = GetHandName(table);
                    if (handName > bestHandName)
                    {
                        bestHand = new Hand(table.ToArray());
                        bestHandName = handName;
                    }
                }
            }

            return bestHand;
        }

        private static readonly Lazy<Swap[]> turnSwaps
            = new Lazy<Swap[]>(() => Enumerable
                .Range(0, 5)
                .Select(i => new Swap(0, i))
                .ToArray());

        private Hand CombineTurn(Card[] table)
        {
            var bestHand = new Hand(cards[1], table[0], table[1], table[2], table[3]);
            var bestHandName = bestHand.Name;
            
            foreach (var swap in turnSwaps.Value)
            {
                using (new TemporarySwap(table, cards, swap))
                {
                    var handName = GetHandName(table);
                    if (handName > bestHandName)
                    {
                        bestHandName = handName;
                        bestHand = new Hand(table.ToArray());
                    }
                }
            }

            return bestHand;
        }

        #endregion

        #region HandName

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
                if (current - ranks[i] != 1)
                    return false;

                current = ranks[i];
            }

            return true;
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

        #region IEnumerable

        public IEnumerator<Card> GetEnumerator()
        {
            return ((IList<Card>) cards).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return cards.GetEnumerator();
        }

        #endregion
    }
}
