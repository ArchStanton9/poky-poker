using PokyPoker.Desktop.Model;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var gameModel = new GameModel();

            Player1 = new PlayerViewModel(0, gameModel.ObservableGame)
            {
                Name = "Johnny \"The Fox\"",
                PhotoUrl = "Assets/Avatar1.png"
            };

            Player2 = new PlayerViewModel(1, gameModel.ObservableGame)
            {
                Name = "Poker Queen",
                PhotoUrl = "Assets/Avatar2.png"
            };

            Player3 = new PlayerViewModel(2, gameModel.ObservableGame)
            {
                Name = "Gambler #3",
                PhotoUrl = "Assets/Avatar3.png",
            };

            Player4 = new PlayerViewModel(3, gameModel.ObservableGame)
            {
                Name = "Gandalf the grey",
                PhotoUrl = "Assets/Avatar4.png",
            };

            PlayOptionsViewModel = new PlayOptionsViewModel(gameModel);
            ChatViewModel = new ChatViewModel();
            BoardViewModel = new BoardViewModel(gameModel.ObservableGame);
        }

        public BoardViewModel BoardViewModel { get; set; }

        public PlayerViewModel Player1 { get; set; }

        public PlayerViewModel Player2 { get; set; }

        public PlayerViewModel Player3 { get; set; }

        public PlayerViewModel Player4 { get; set; }

        public PlayOptionsViewModel PlayOptionsViewModel { get; set; }

        public ChatViewModel ChatViewModel { get; set; }
    }
}
