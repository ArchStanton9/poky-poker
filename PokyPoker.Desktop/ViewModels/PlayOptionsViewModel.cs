using System.Reactive;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.ViewModels
{
    public class PlayOptionsViewModel : ReactiveObject
    {
        public PlayOptionsViewModel()
        {
            MakePlayCommand = ReactiveCommand.Create<Play>(p =>
            {

            });
            CanFold = true;
            CanCall = true;
            CanRaise = true;
            Bet = 100;
            MinBet = 100;
            MaxBet = 500;
        }

        public ReactiveCommand<Play, Unit> MakePlayCommand;

        [Reactive]
        public int Bet { get; set; }

        [Reactive]
        public int MinBet { get; set; }

        [Reactive]
        public int MaxBet { get; set; }

        [Reactive]
        public bool CanBet { get; set; }

        [Reactive]
        public bool CanRaise { get; set; }

        [Reactive]
        public bool CanCall { get; set; }

        [Reactive]
        public bool CanAllIn { get; set; }

        [Reactive]
        public bool CanCheck { get; set; }

        [Reactive]
        public bool CanFold { get; set; }
    }
}
