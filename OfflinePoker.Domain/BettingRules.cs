namespace OfflinePoker.Domain
{
    public class BettingRules
    {
        public int SmallBlind { get; set; }

        public int BigBlind { get; set; }

        public static readonly BettingRules Standard = new BettingRules()
        {
            SmallBlind = 20,
            BigBlind = 40
        };
    }
}
