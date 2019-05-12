namespace PokyPoker.Desktop.PlayActions
{
    public interface IPlayWithBet : IPlayViewModel
    {
        int Bet { get; set; }
    }
}
