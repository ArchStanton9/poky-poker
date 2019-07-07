using System.Reactive.Disposables;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for BoardView.xaml
    /// </summary>
    public partial class BoardView : ReactiveUserControl<BoardViewModel>
    {
        public BoardView()
        {
            InitializeComponent();
            this.WhenActivated(cleanUp =>
            {
                this.OneWayBind(ViewModel, vm => vm.BoardCards, v => v.BoardCardsList.ItemsSource)
                    .DisposeWith(cleanUp);
            });
        }
    }
}
