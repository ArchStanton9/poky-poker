using System.Reactive;
using PokyPoker.Desktop.Models;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.ViewModels
{
    public class BetViewModel : IPlayWithBet
    {
        public BetViewModel(Play betPlay, GameModel model)
        {
            Play = betPlay;
            ActCommand = ReactiveCommand.Create(() => model.MakeAct(Play, Bet));
        }

        public Play Play { get; }
        public string DisplayName => Play.ToString();
        public ReactiveCommand<Unit, Unit> ActCommand { get; }

        [Reactive]
        public int Bet { get; set; }
    }
}
