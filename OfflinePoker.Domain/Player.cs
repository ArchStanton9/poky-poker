namespace OfflinePoker.Domain
{
    public class Player
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

        public Player Deactivate()
        {
            return new Player(Name, Hand, !IsActive, Stack);
        }
    }
}
