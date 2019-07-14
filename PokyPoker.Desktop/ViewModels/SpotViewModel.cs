using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class SpotViewModel : ReactiveObject
    {
        public SpotViewModel(int spotNumber, ReadOnlyObservableCollection<PlayerViewModel> players)
        {
            SpotNumber = spotNumber;

            players
                .ToObservableChangeSet()
                .AutoRefresh()
                .Filter(p => p.Spot == spotNumber)
                .ToCollection()
                .Select(c => c.SingleOrDefault())
                .ToProperty(this, vm => vm.PlayerViewModel, out playerViewModel);
            
            this.WhenAnyValue(vm => vm.PlayerViewModel)
                .Select(p => p == null)
                .ToProperty(this, vm => vm.IsEmpty, out isEmpty);
        }

        public int SpotNumber { get; set; }

        private readonly ObservableAsPropertyHelper<PlayerViewModel> playerViewModel;
        public PlayerViewModel PlayerViewModel => playerViewModel.Value;

        private readonly ObservableAsPropertyHelper<bool> isEmpty;
        public bool IsEmpty => isEmpty.Value;
    }
}
