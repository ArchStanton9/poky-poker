namespace PokyPoker.Domain
{
    public struct Act
    {
        public Act(int player, Play play, int bet)
        {
            Player = player;
            Play = play;
            Bet = bet;
        }

        public int Player { get; }
        public Play Play { get; }
        public int Bet { get; }
    }
}