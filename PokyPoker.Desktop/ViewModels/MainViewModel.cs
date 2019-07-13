using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public BoardViewModel BoardViewModel { get; set; } = new BoardViewModel();

        public PlayerViewModel Player { get; set; } = new PlayerViewModel();
    }
}
