using System;

namespace PokyPoker.Domain
{
    public class Player
    {
        public Player(int id, Hand hand, bool active, int stack)
        {
            Id = id;
            Hand = hand;
            IsActive = active;
            Stack = stack;
        }

        public int Id { get; }
        public Hand Hand { get; }
        public bool IsActive { get; }
        public int Stack { get; }

        public Player WithHand(Hand hand)
        {
            return new Player(Id, hand, IsActive, Stack);
        }

        public Player WithHand(Func<Hand, Hand> handFunc)
        {
            return new Player(Id, handFunc(Hand), IsActive, Stack);
        }

        public Player WithStack(int stack)
        {
            if (stack == Stack)
                return this;

            return new Player(Id, Hand, IsActive, stack);
        }

        public Player WithStack(Func<int, int> stackFunc)
        {
            var stack = stackFunc(Stack);
            return WithStack(stack);
        }

        public Player MakeInactive()
        {
            return new Player(Id, Hand, !IsActive, Stack);
        }
    }
}
