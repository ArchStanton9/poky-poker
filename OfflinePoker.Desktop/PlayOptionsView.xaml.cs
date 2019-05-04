using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using OfflinePoker.Domain;
using ReactiveUI;

namespace OfflinePoker.Desktop
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

                this.Bind(ViewModel, v => v.Bet, v => v.BetTextBox.Text)
                    .DisposeWith(cleanup);
            });
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && Enum.TryParse<Play>(button.Content.ToString(), out var play))
            {
                ViewModel.MakePlayCommand.Execute(play).Subscribe();
            }
        }
    }
}
