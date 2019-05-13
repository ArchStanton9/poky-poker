using System.Reactive.Disposables;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for BetPlayView.xaml
    /// </summary>
    public partial class BetPlayView : ReactiveUserControl<IPlayWithBet>
    {
        public BetPlayView()
        {
            InitializeComponent();

            this.WhenActivated(cleanUp =>
            {
                this.BindCommand(ViewModel, vm => vm.ActCommand, v => v.ActButton)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.ActButton.Content)
                    .DisposeWith(cleanUp);

                this.Bind(ViewModel, vm => vm.Bet, v => v.BetTextBox.Text)
                    .DisposeWith(cleanUp);
            });
        }
    }
}
