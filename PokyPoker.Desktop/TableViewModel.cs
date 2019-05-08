using System;
using System.Reactive.Linq;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop
{
    public class TableViewModel : ReactiveObject
    {
        public TableViewModel(IObservable<Game> observableGame)
        {
            observableGame
                .Select(g => g.Pot)
                .ToProperty(this, vm => vm.Pot, out pot);

            observableGame
                .Select(g => HandFormatter.Format(g.Table))
                .ToProperty(this, vm => vm.Table, out table);
        }

        public int Pot => pot.Value;
        private readonly ObservableAsPropertyHelper<int> pot;

        public string Table => table.Value;
        private readonly ObservableAsPropertyHelper<string> table;
    }
}
