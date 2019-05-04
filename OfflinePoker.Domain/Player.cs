using System;

namespace OfflinePoker.Domain
{
    public struct Player
    {
        public Player(string name, Hand hand, bool active, int stack)
        {
            Name = name;
            Hand = hand;
            IsActive = active;
            Stack = stack;
        }

        public string Name { get; }
        public Hand Hand { get; }
        public bool IsActive { get; }
        public int Stack { get; }

        public Player WithHand(Hand hand)
        {
            return new Player(Name, hand, IsActive, Stack);
        }

        public Player WithStack(int stack)
        {
            if (stack == Stack)
                return this;

            return new Player(Name, Hand, IsActive, stack);
        }

        public Player WithStack(Func<int, int> stackFunc)
        {
            var stack = stackFunc(Stack);
            return WithStack(stack);
        }

        public Player MakeInactive()
        {
            return new Player(Name, Hand, !IsActive, Stack);
        }
    }
}
