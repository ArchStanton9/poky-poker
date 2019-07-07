using System;
using System.Reactive.Disposables;
using System.Windows;
using PokyPoker.Client;
using ReactiveUI;

namespace PokyPoker.Desktop.Dealer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(cleanUp =>
            {
                this.OneWayBind(ViewModel, vm => vm.RoomBuilder, v => v.RoomBuilderView.ViewModel)
                    .DisposeWith(cleanUp);
            });

            ViewModel = new MainViewModel();
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }

        public MainViewModel ViewModel { get; set; }
    }
}
