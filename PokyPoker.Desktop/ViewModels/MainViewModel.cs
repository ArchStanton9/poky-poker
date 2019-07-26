using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using PokyPoker.Desktop.Model;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public TimeSpan ShowdownDelay { get; set; } = TimeSpan.FromSeconds(8);

        public MainViewModel()
        {
            var gameModel = new GameModel();

            Spots = new ObservableCollection<SpotViewModel>();

            var playersSource = gameModel.Players
                .Transform(p => new PlayerViewModel(p, gameModel.ObservableGame))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out players)
                .Subscribe();

            Spots.AddRange(Enumerable.Range(0, 8).Select(i => new SpotViewModel(i, Players)));


            PlayOptionsViewModel = new PlayOptionsViewModel(gameModel);
            ChatViewModel = new ChatViewModel(Players);
            BoardViewModel = new BoardViewModel(gameModel.ObservableGame);

            gameModel.ObservableGame
                .Select(g => g.IsComplete)
                .Where(g => g)
                .Delay(ShowdownDelay)
                .ObserveOnDispatcher()
                .Do(b => gameModel.StartNext())
                .Subscribe();
        }

        private readonly ReadOnlyObservableCollection<PlayerViewModel> players;
        public ReadOnlyObservableCollection<PlayerViewModel> Players => players;

        public BoardViewModel BoardViewModel { get; set; }

        public ObservableCollection<SpotViewModel> Spots { get; set; }

        public PlayOptionsViewModel PlayOptionsViewModel { get; set; }

        public ChatViewModel ChatViewModel { get; set; }
    }
}
