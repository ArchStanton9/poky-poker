using System.Reactive.Disposables;
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

                this.OneWayBind(ViewModel, vm => vm.Pot, v => v.PotTextBox.Text, x => $"Pot : {x}")
                    .DisposeWith(cleanup);
            });
        }
    }
}
