namespace OfflinePoker.Domain
{
    public struct RoundState
    {
        public bool IsActive { get; set; }
        public bool IsCurrent { get; set; }
        public int Bet { get; set; }
        public Play LastPlay { get; set; }

        public static readonly RoundState Inactive = new RoundState();
    }
}
