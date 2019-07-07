using System.Reactive.Disposables;
using ReactiveUI;

namespace PokyPoker.Desktop.Dealer
{
    /// <summary>
    /// Interaction logic for RoomBuilderView.xaml
    /// </summary>
    public partial class RoomBuilderView : ReactiveUserControl<RoomBuilderViewModel>
    {
        public RoomBuilderView()
        {
            InitializeComponent();

            this.WhenActivated(cleanUp =>
            {
                this.Bind(ViewModel, vm => vm.BigBlind, v => v.BigBlindTextBox.Text)
                    .DisposeWith(cleanUp);

                this.Bind(ViewModel, vm => vm.SmallBlind, v => v.SmallBlindTextBox.Text)
                    .DisposeWith(cleanUp);

                this.Bind(ViewModel, vm => vm.PlayersCount, v => v.PlayersTextBox.Text)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.ErrorMessage, v => v.ErrorTextBlock.Text)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.CreateRoomCommand, v => v.CreateRoomButton.Command)
                    .DisposeWith(cleanUp);
            });
        }
    }
}
