using System.Reactive.Disposables;
using PokyPoker.Desktop.TestApp.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.TestApp.Views
{
    /// <summary>
    /// Interaction logic for HandView.xaml
    /// </summary>
    public partial class HandView : ReactiveUserControl<HandViewModel>
    {
        public HandView()
        {
            InitializeComponent();

            this.WhenActivated(cleanUp =>
            {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.NameTextBlock.Text)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.Cards, v => v.CardsTextBlock.Text)
                    .DisposeWith(cleanUp);
            });
        }
    }
}
