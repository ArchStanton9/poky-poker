using System.Reactive.Disposables;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.Views
{
    /// <summary>
    /// Interaction logic for PlayOptionsView.xaml
    /// </summary>
    public partial class PlayOptionsView : ReactiveUserControl<PlayOptionsViewModel>
    {
        public PlayOptionsView()
        {
            InitializeComponent();
            this.WhenActivated(cleanup =>
            {
                this.OneWayBind(ViewModel, v => v.Options, v => v.PlaysList.ItemsSource)
                    .DisposeWith(cleanup);

            });
        }
    }
}
