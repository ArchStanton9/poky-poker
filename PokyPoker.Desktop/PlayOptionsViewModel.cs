using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop
{
    public class PlayOptionsViewModel : ReactiveObject
    {
        private readonly SourceList<Play> optionsSource = new SourceList<Play>();

        public PlayOptionsViewModel(IObservable<Game> observableGame, Action<Play, int> makePlayFunc)
        {
            optionsSource.Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out options)
                .Subscribe();

            observableGame.Subscribe(game =>
            {
                var round = game.CurrentRound;
                Bet = round.MaxBet - round.PlayerBet(game.CurrentPlayer.Id);

                optionsSource.Clear();
                var opt = game.GetOptions();
                if (opt.Any())
                    optionsSource.AddRange(opt);
            });

            MakePlayCommand = ReactiveCommand
                .Create<Play>(p => makePlayFunc(p, Bet));
        }

        [Reactive]
        public int Bet { get; set; }

        public ReadOnlyObservableCollection<Play> Options => options;
        private readonly ReadOnlyObservableCollection<Play> options;

        public ReactiveCommand<Play, Unit> MakePlayCommand { get; set; }
    }
}
