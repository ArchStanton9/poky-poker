using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var model = new GameModel();

            Players = new ObservableCollection<PlayerViewModel>(model.Players.Select(p =>
                new PlayerViewModel(p.Id, model.ObservableGame)));

            Table = new TableViewModel(model.ObservableGame);
            PlayOptionsViewModel = new PlayOptionsViewModel(model);
        }

        public ObservableCollection<PlayerViewModel> Players { get; }

        [Reactive]
        public TableViewModel Table { get; set; }

        [Reactive]
        public PlayOptionsViewModel PlayOptionsViewModel { get; set; }
    }
}
