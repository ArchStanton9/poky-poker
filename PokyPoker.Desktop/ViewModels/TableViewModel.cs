using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class TableViewModel : ReactiveObject
    {
        public TableViewModel(IObservable<Game> observableGame)
        {
            observableGame
                .Select(g => g.MainPot.Amount)
                .ToProperty(this, vm => vm.Pot, out pot);

            SidePots = new ObservableCollection<string>();
            observableGame
                .Select(g => g.SidePots)
                .Subscribe(s =>
                {
                    SidePots.Clear();
                    SidePots.AddRange(s.Select(p => p.Amount.ToString()));
                });

            observableGame
                .Select(g => g.SidePots.Any())
                .ToProperty(this, vm => vm.IsSidePotsVisible, out isSidePotsVisible);

            observableGame
                .Select(g => HandFormatter.Format(g.CurrentTable))
                .ToProperty(this, vm => vm.Table, out table);
        }

        public int Pot => pot.Value;
        private readonly ObservableAsPropertyHelper<int> pot;

        public ObservableCollection<string> SidePots { get; }

        public bool IsSidePotsVisible => isSidePotsVisible.Value;
        private readonly ObservableAsPropertyHelper<bool> isSidePotsVisible;

        public string Table => table.Value;
        private readonly ObservableAsPropertyHelper<string> table;
    }
}
