namespace OfflinePoker.Domain
{
    public class Player
    {
        public Player(string name, Hand hand, int position, int stack)
        {
            Name = name;
            Hand = hand;
            Position = position;
            Stack = stack;
        }

        public string Name { get; }
        public Hand Hand { get; }
        public int Position { get; }
        public int Stack { get; }

        public Player WithHand(Hand hand)
        {
            return new Player(Name, hand, Position, Stack);
        }
    }
}
