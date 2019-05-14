namespace PokyPoker.Contracts.Requests
{
    public class CreateRoomRequest
    {
        public int PlayersCount { get; set; }

        public BettingRulesDto Rules { get; set; }
    }
}
