using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using PokyPoker.Desktop.TestApp.Models;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.TestApp.ViewModels
{
    public class PlayOptionsViewModel : ReactiveObject
    {
        public PlayOptionsViewModel(GameModel gameModel)
        {
            var optionsSource = new IPlayViewModel[]
            {
                new BetViewModel(Play.Bet, gameModel),
                new BetViewModel(Play.Raise, gameModel),
                new CallViewModel(gameModel),
                new PlayViewModel(Play.AllIn, gameModel),
                new PlayViewModel(Play.Check, gameModel),
                new PlayViewModel(Play.Fold, gameModel),
            };

            var comparer = Comparer<IPlayViewModel>.Create((l, r) => - l.Play.CompareTo(r.Play));
            var filter = gameModel.ObservableGame.Select(game => VisiblePredicate(game.GetOptions()));
            optionsSource.AsObservableChangeSet()
                .Filter(filter)
                .Sort(comparer)
                .Bind(out options)
                .Subscribe();
        }

        private static Func<IPlayViewModel, bool> VisiblePredicate(IEnumerable<Play> options)
        {
            return vm => options.Contains(vm.Play);
        }

        public ReadOnlyObservableCollection<IPlayViewModel> Options => options;
        private readonly ReadOnlyObservableCollection<IPlayViewModel> options;
    }
}
