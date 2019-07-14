namespace PokyPoker.Domain
{
    public struct PlayerState
    {
        public bool IsActive { get; set; }
        public bool ShouldAct { get; set; }
        public int Bet { get; set; }
        public Play LastPlay { get; set; }

        public static readonly PlayerState Inactive = new PlayerState();
    }
}
