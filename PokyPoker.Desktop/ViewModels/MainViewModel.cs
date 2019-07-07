using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public BoardViewModel BoardViewModel { get; set; } = new BoardViewModel();
    }
}
