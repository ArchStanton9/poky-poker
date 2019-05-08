using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PokyPoker.Domain.Exceptions;

namespace PokyPoker.Domain
{
    public class Hand : IEnumerable<Card>, IComparable<Hand>, IComparable
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

        #region Combine

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
            var ranks = new byte[cards.Length];
            for (var i = 0; i < cards.Length; i++)
            {
                ranks[i] = (byte) cards[i].Rank;
            }

            Array.Sort(ranks);
            Array.Reverse(ranks);

            if (ranks.Length == 5)
            {
                var flash = IsFlush(cards);
                var straight = IsStraight(ranks);

                if (flash && straight) return HandName.StraightFlush;
                if (flash) return HandName.Flush;
                if (straight) return HandName.Straight;
            }

            return CheckNameByLayout(new ArraySegment<byte>(ranks));
        }


        private static bool IsFlush(IReadOnlyList<Card> cards)
        {
            var current = cards[0].Suit;
            for (int i = 1; i < cards.Count; i++)
            {
                if (current != cards[i].Suit)
                    return false;
            }

            return true;
        }
        
        private static bool IsStraight(byte[] ranks)
        {

#if DEBUG
            AssertSorted(ranks);
#endif

            var current = ranks[0];
            for (var i = 1; i < ranks.Length; i++)
            {
                if (current - ranks[i] != 1)
                {
                    const byte ace = (byte) Rank.Ace;
                    const byte five = (byte) Rank.Five;
                    if (current != ace || ranks[i] != five)
                        return false;
                }

                current = ranks[i];
            }

            return true;
        }

        private static HandName CheckNameByLayout(ArraySegment<byte> ranks)
        {
#if DEBUG
            AssertSorted(ranks.Array);
#endif

            var layout = ParseLayout(ranks);
            return layoutsMap.TryGetValue(layout, out var name) ? name : HandName.HighCard;
        }

        private static readonly Dictionary<int, HandName> layoutsMap = new Dictionary<int, HandName>
        {
            {2111, HandName.OnePair},
            {1211, HandName.OnePair},
            {1121, HandName.OnePair},
            {1112, HandName.OnePair},
            {211, HandName.OnePair},
            {121, HandName.OnePair},
            {112, HandName.OnePair},
            {21, HandName.OnePair},
            {12, HandName.OnePair},
            {2, HandName.OnePair},
            
            {221, HandName.TwoPair},
            {212, HandName.TwoPair},
            {122, HandName.TwoPair},
            {22, HandName.TwoPair},

            {311, HandName.ThreeOfAKind},
            {131, HandName.ThreeOfAKind},
            {113, HandName.ThreeOfAKind},
            {13, HandName.ThreeOfAKind},
            {31, HandName.ThreeOfAKind},
            {3, HandName.ThreeOfAKind},
            
            {41, HandName.FourOfAKind},
            {14, HandName.FourOfAKind},
            {4, HandName.FourOfAKind},

            {32, HandName.FullHouse},
            {23, HandName.FullHouse}
        };

        private static int ParseLayout(ArraySegment<byte> segment)
        {
            var result = 0;

            for (var i = 0; i < 5; i++)
            {
                if (segment.Count == 0)
                    return result;

                segment = Take(segment, out var c);
                result += c * (int) Math.Pow(10, i);

                if (segment.Count < 1)
                    break;
            }

            return result;
        }

        private static ArraySegment<byte> Take(ArraySegment<byte> segment, out int count)
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

        private static int NextEdge(ArraySegment<byte> segment)
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

        #region IComparable

        public int CompareTo(Hand other)
        {
            var result = CompareByHandNames(this, other);
            return result != 0 ? result : CompareByKickers(this, other);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (ReferenceEquals(this, obj)) return 0;
            return obj is Hand other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Hand)}");
        }

        public static bool operator <(Hand left, Hand right)
        {
            return Comparer<Hand>.Default.Compare(left, right) < 0;
        }

        public static bool operator >(Hand left, Hand right)
        {
            return Comparer<Hand>.Default.Compare(left, right) > 0;
        }

        public static bool operator <=(Hand left, Hand right)
        {
            return Comparer<Hand>.Default.Compare(left, right) <= 0;
        }

        public static bool operator >=(Hand left, Hand right)
        {
            return Comparer<Hand>.Default.Compare(left, right) >= 0;
        }

        private static int CompareByHandNames(Hand left, Hand right)
        {
            if (left.Name > right.Name)
                return 1;
            if (left.Name == right.Name)
                return 0;

            return -1;
        }

        private static int CompareByKickers(Hand left, Hand right)
        {
            if (left.Name != right.Name)
                throw new GameLogicException("Can't compare hands by kickers. Names doesn't match.");

            var l = left.GetKickers();
            var r = right.GetKickers();

            if (l.Length != r.Length)
                throw new GameLogicException("Can't compare hands. Kicker sequences doesn't match.");

            for (var i = 0; i < l.Length; i++)
            {
                if (l[i] == r[i])
                    continue;

                return l[i] > r[i] ? 1 : -1;
            }

            return 0;
        }

        private byte[] GetKickers()
        {
            var ranks = cards
                .Select(c => (byte)c.Rank)
                .OrderByDescending(x => x)
                .ToArray();

            switch (Name)
            {
                case HandName.Straight:
                case HandName.Flush:
                case HandName.StraightFlush:
                    return new[] { ranks[0] };

                case HandName.FourOfAKind:
                    return FourOfKindKickers(ranks);

                case HandName.FullHouse:
                    return new[] { ranks[2] };

                case HandName.ThreeOfAKind:
                    return ThreeOfKindKickers(ranks);

                case HandName.TwoPair:
                    return TwoPairsKickers(ranks);

                case HandName.OnePair:
                    return OnePairKickers(ranks);
            }

            return ranks;
        }

        private static byte[] FourOfKindKickers(IReadOnlyList<byte> ranks)
        {
            if (ranks.Count == 4)
                return new[] {ranks[0]};

            return ranks[0] == ranks[1]
                ? new[] {ranks[0], ranks[4]}
                : new[] {ranks[4], ranks[0]};
        }

        private static byte[] ThreeOfKindKickers(byte[] ranks)
        {
            var layout = ParseLayout(new ArraySegment<byte>(ranks));
            switch (layout)
            {
                case 311:
                    return new[] {ranks[0], ranks[3], ranks[4]};
                case 131:
                    return new[] {ranks[1], ranks[0], ranks[4]};
                case 113:
                    return new[] {ranks[2], ranks[0], ranks[1]};
                case 31:
                    return new[] {ranks[0], ranks[3]};
                case 13:
                    return new[] {ranks[3], ranks[0]};
                case 3:
                    return new[] {ranks[0]};
                default:
                    throw new GameLogicException($"Unexpected layout '{layout}' for ThreeOfKind.");
            }
        }

        private static byte[] TwoPairsKickers(byte[] ranks)
        {
            var layout = ParseLayout(new ArraySegment<byte>(ranks));
            switch (layout)
            {
                case 221:
                    return new[] {ranks[0], ranks[2], ranks[4]};
                case 212:
                    return new[] {ranks[0], ranks[4], ranks[2]};
                case 122:
                    return new[] {ranks[2], ranks[4], ranks[0]};
                case 22:
                    return new[] {ranks[0], ranks[2]};
                default:
                    throw new GameLogicException($"Unexpected layout '{layout}' for TwoPairs.");
            }
        }

        private static byte[] OnePairKickers(byte[] ranks)
        {
            var layout = ParseLayout(new ArraySegment<byte>(ranks));
            switch (layout)
            {
                case 2111:
                    return new[] {ranks[0], ranks[2], ranks[3], ranks[4]};
                case 1211:
                    return new[] {ranks[1], ranks[0], ranks[3], ranks[4]};
                case 1121:
                    return new[] {ranks[2], ranks[0], ranks[1], ranks[4]};
                case 1112:
                    return new[] {ranks[3], ranks[0], ranks[1], ranks[2]};

                case 211:
                    return new[] {ranks[0], ranks[2], ranks[3]};
                case 121:
                    return new[] {ranks[1], ranks[0], ranks[3]};
                case 112:
                    return new[] {ranks[0], ranks[1], ranks[2]};

                case 21:
                    return new[] {ranks[0], ranks[2]};
                case 12:
                    return new[] {ranks[2], ranks[0]};

                case 2:
                    return new[] {ranks[0]};
                default:
                    throw new GameLogicException($"Unexpected layout '{layout}' for OnePair.");
            }
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
