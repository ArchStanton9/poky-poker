using System.Reactive.Disposables;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView : ReactiveUserControl<PlayerViewModel>
    {
        public PlayerView()
        {
            InitializeComponent();
            this.WhenActivated(cleanUp =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.PlayerNameTextBlock.Text)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.Currency, v => v.PlayerCurrency.Text)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.PhotoUrl, v => v.PlayerImage.Source)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.LeftCard, v => v.LeftCardView.Card)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.RightCard, v => v.RightCardView.Card)
                    .DisposeWith(cleanUp);
            });
        }
    }
}
