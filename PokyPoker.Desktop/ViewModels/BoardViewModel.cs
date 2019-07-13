using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class BoardViewModel : ReactiveObject
    {
        public BoardViewModel(IObservable<Game> game)
        {
            BoardCards = new ObservableCollectionExtended<Card>();
            game
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(g =>
                {
                    BoardCards.Clear();
                    BoardCards.AddRange(g.CurrentTable);
                });

            game
                .Select(g => g.MainPot.Amount)
                .ToProperty(this, vm => vm.Pot, out pot);

            SidePots = new ObservableCollection<string>();
            game
                .Select(g => g.SidePots)
                .Subscribe(s =>
                {
                    SidePots.Clear();
                    SidePots.AddRange(s.Select(p => p.Amount.ToString()));
                });

            game
                .Select(g => g.SidePots.Any())
                .ToProperty(this, vm => vm.IsSidePotsVisible, out isSidePotsVisible);
        }

        public ObservableCollection<Card> BoardCards { get; set; }

        public int Pot => pot.Value;
        private readonly ObservableAsPropertyHelper<int> pot;

        public ObservableCollection<string> SidePots { get; }

        public bool IsSidePotsVisible => isSidePotsVisible.Value;
        private readonly ObservableAsPropertyHelper<bool> isSidePotsVisible;

    }
}
