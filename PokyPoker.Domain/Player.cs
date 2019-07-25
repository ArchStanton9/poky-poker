using System;

namespace PokyPoker.Domain
{
    public struct Player : IEquatable<Player>
    {
        public Player(int spot, Hand hand, bool isActive, int stack)
        {
            Spot = spot;
            Hand = hand;
            IsActive = isActive;
            Stack = stack;
        }

        public int Spot { get; }
        public Hand Hand { get; }
        public bool IsActive { get; }
        public int Stack { get; }

        public Player WithHand(Hand hand)
        {
            return new Player(Spot, hand, IsActive, Stack);
        }

        public Player WithHand(Func<Hand, Hand> handFunc)
        {
            return new Player(Spot, handFunc(Hand), IsActive, Stack);
        }

        public Player WithStack(int stack)
        {
            if (stack == Stack)
                return this;

            return new Player(Spot, Hand, IsActive, stack);
        }

        public Player WithStack(Func<int, int> stackFunc)
        {
            var stack = stackFunc(Stack);
            return WithStack(stack);
        }

        public Player MakeInactive()
        {
            return new Player(Spot, Hand, !IsActive, Stack);
        }

        public bool Equals(Player other)
        {
            return Spot == other.Spot;
        }

        public override bool Equals(object obj)
        {
            return obj is Player other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Spot;
        }

        public static bool operator ==(Player left, Player right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Player left, Player right)
        {
            return !left.Equals(right);
        }
    }
}
