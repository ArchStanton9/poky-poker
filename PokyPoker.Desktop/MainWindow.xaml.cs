using System.Reactive.Disposables;
using System.Windows;
using PokyPoker.Desktop.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(cleanup =>
            {
                this.OneWayBind(ViewModel, v => v.Players, v => v.PlayersList.ItemsSource)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                        v => v.PlayOptionsViewModel,
                        v => v.PlayOptionsView.ViewModel)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel,
                        vm => vm.Table,
                        v => v.TableView.ViewModel)
                    .DisposeWith(cleanup);

            });

            ViewModel = new MainViewModel();
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(MainViewModel), typeof(MainWindow), new PropertyMetadata(default(MainViewModel)));

        public MainViewModel ViewModel
        {
            get => (MainViewModel) GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel) value;
        }
    }
}
