using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media;
using PokyPoker.Desktop.ViewModels;
using PokyPoker.Domain;
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

                this.OneWayBind(ViewModel,
                    vm => vm.PlayerState,
                    v => v.PlayerBorder.BorderBrush,
                    x => x.ShouldAct ? Brushes.DodgerBlue : Brushes.DarkGray);

                this.OneWayBind(ViewModel,
                        vm => vm.PlayerState, v => v.LastPlayLabel.Content,
                        s => $"{s.LastPlay} {s.Bet}")
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel,
                        vm => vm.PlayerState, v => v.LastPlayLabel.Visibility,
                        s => s.LastPlay == Play.None ? Visibility.Hidden : Visibility.Visible)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel,
                    vm => vm.PlayerState, v => v.HandView.Visibility,
                    s =>
                    {
                        return s.IsActive ? Visibility.Visible : Visibility.Hidden;
                    });
            });
        }
    }
}
