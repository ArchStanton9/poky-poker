using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for SpotView.xaml
    /// </summary>
    public partial class SpotView : ReactiveUserControl<SpotViewModel>
    {
        public SpotView()
        {
            InitializeComponent();

            this.WhenActivated(cleanUp =>
            {

                this.OneWayBind(ViewModel, vm => vm.IsEmpty, v => v.Rect.Visibility,
                    e => !e ? Visibility.Collapsed : Visibility.Visible)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.IsEmpty, v => v.PlayerView.Visibility,
                        e => e ? Visibility.Collapsed : Visibility.Visible)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.PlayerViewModel, v => v.PlayerView.ViewModel)
                    .DisposeWith(cleanUp);

            });
        }
    }
}
