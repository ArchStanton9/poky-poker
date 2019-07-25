using System.Reactive.Disposables;
using System.Windows;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for TableSpotView.xaml
    /// </summary>
    public partial class TableSpotView : ReactiveUserControl<SpotViewModel>
    {
        public TableSpotView()
        {
            InitializeComponent();

            this.WhenActivated(cleanUp =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.PlayerViewModel,
                        v => v.DealerButton.Visibility,
                        p => p != null ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel,
                        vm => vm.PlayerViewModel.IsDealer,
                        v => v.DealerButton.Visibility,
                        d => d ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(cleanUp);
            });
        }
    }
}
