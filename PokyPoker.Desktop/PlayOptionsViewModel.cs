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
                optionsSource.Clear();
                var opt = game.GetOptions();
                if (opt.Any())
                    optionsSource.AddRange(opt);
            });

            observableGame.Select(GetSuggestedBet).Subscribe(b => Bet = b);

            MakePlayCommand = ReactiveCommand.Create<Play>(p => makePlayFunc(p, Bet));
        }

        private static int GetSuggestedBet(Game game)
        {
            var round = game.CurrentRound;
            var toCall = round.MaxBet - round.PlayerBet(game.CurrentPlayer.Id);

            return Math.Min(toCall, game.CurrentPlayer.Stack);
        }

        [Reactive]
        public int Bet { get; set; }

        public ReadOnlyObservableCollection<Play> Options => options;
        private readonly ReadOnlyObservableCollection<Play> options;

        public ReactiveCommand<Play, Unit> MakePlayCommand { get; set; }
    }
}
