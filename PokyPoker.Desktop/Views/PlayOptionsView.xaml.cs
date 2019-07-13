using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using PokyPoker.Desktop.ViewModels;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for PlayOptionsView.xaml
    /// </summary>
    public partial class PlayOptionsView : ReactiveUserControl<PlayOptionsViewModel>
    {
        private Visibility ToVisibility(bool visible) =>
            visible ? Visibility.Visible : Visibility.Collapsed;

        public PlayOptionsView()
        {
            InitializeComponent();
            this.WhenActivated(cleanUp =>
            {

                #region Command Bindings

                this.OneWayBind(ViewModel, vm => vm.CanBet, v => v.BetButton.Visibility, ToVisibility)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.MakePlayCommand,
                        v => v.BetButton, Observable.Return(Play.Bet))
                    .DisposeWith(cleanUp);


                this.OneWayBind(ViewModel, vm => vm.CanRaise, v => v.RaiseButton.Visibility, ToVisibility)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.MakePlayCommand,
                        v => v.RaiseButton, Observable.Return(Play.Raise))
                    .DisposeWith(cleanUp);

                
                this.OneWayBind(ViewModel, vm => vm.CanAllIn, v => v.AllInButton.Visibility, ToVisibility)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.MakePlayCommand,
                        v => v.AllInButton, Observable.Return(Play.AllIn))
                    .DisposeWith(cleanUp);

                
                this.OneWayBind(ViewModel, vm => vm.CanCall, v => v.CallButton.Visibility, ToVisibility)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.MakePlayCommand,
                        v => v.CallButton, Observable.Return(Play.Call))
                    .DisposeWith(cleanUp);

                
                this.OneWayBind(ViewModel, vm => vm.CanCheck, v => v.CheckButton.Visibility, ToVisibility)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.MakePlayCommand,
                        v => v.CheckButton, Observable.Return(Play.Check))
                    .DisposeWith(cleanUp);


                this.OneWayBind(ViewModel, vm => vm.CanFold, v => v.FoldButton.Visibility, ToVisibility)
                    .DisposeWith(cleanUp);

                this.BindCommand(ViewModel, vm => vm.MakePlayCommand,
                        v => v.FoldButton, Observable.Return(Play.Fold))
                    .DisposeWith(cleanUp);

                #endregion

                this.OneWayBind(ViewModel, vm => vm.Bet, v => v.BetLabel.Content)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.MinBet, v => v.BetSlider.Minimum)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.MaxBet, v => v.BetSlider.Maximum)
                    .DisposeWith(cleanUp);

                this.Bind(ViewModel, vm => vm.Bet, v => v.BetSlider.Value)
                    .DisposeWith(cleanUp);


            });
        }
    }
}
