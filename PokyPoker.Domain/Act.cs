namespace PokyPoker.Domain
{
    public struct Act
    {
        public Act(Player player, Play play, int bet)
        {
            Player = player;
            Play = play;
            Bet = bet;
        }

        public Player Player { get; }
        public Play Play { get; }
        public int Bet { get; }
    }
}