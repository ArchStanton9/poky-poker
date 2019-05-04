using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ReactiveUI;

namespace OfflinePoker.Desktop
{
    /// <summary>
    /// Interaction logic for PlayerView.xaml
    /// </summary>
    public partial class PlayerView : ReactiveUserControl<PlayerViewModel>
    {

        public PlayerView()
        {
            InitializeComponent();

            this.WhenActivated(cleanup =>
            {
                this.Bind(ViewModel, v => v.Name, v => v.NameTextBox.Text)
                    .DisposeWith(cleanup);

                this.Bind(ViewModel, v => v.Bank, v => v.BankTextBox.Text)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                    vm => vm.RoundState.IsCurrent,
                    v => v.PlayerBorder.BorderBrush,
                    x => x ? Brushes.DodgerBlue: Brushes.DarkGray);

                this.OneWayBind(ViewModel,
                        vm => vm.RoundState.IsActive,
                        v => v.IsActiveCheckbox.IsChecked)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                        vm => vm.RoundState.LastPlay,
                        v => v.LastPlayBlock.Text)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                        vm => vm.RoundState.Bet,
                        v => v.BetTextBlock.Text)
                    .DisposeWith(cleanup);
            });
        }
    }
}
