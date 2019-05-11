using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace PokyPoker.Desktop
{
    /// <summary>
    /// Interaction logic for TableView.xaml
    /// </summary>
    public partial class TableView : ReactiveUserControl<TableViewModel>
    {
        public TableView()
        {
            InitializeComponent();

            this.WhenActivated(cleanup =>
            {
                this.OneWayBind(ViewModel, vm => vm.Table, v => v.TableTextBox.Text)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel, vm => vm.SidePots, v => v.SidePotsList.ItemsSource)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel, vm => vm.IsSidePotsVisible, v => v.SidePotsLabel.Visibility,
                        sp => sp ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel, vm => vm.Pot, v => v.PotTextBox.Text, x => $"Pot : {x}")
                    .DisposeWith(cleanup);
            });
        }
    }
}
