using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using OfflinePoker.Domain;
using ReactiveUI;

namespace OfflinePoker.Desktop
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

                this.OneWayBind(ViewModel, v => v.Options, v => v.PlaysList.ItemsSource)
                    .DisposeWith(cleanup);

                this.Bind(ViewModel, v => v.Bet, v => v.BetTextBox.Text)
                    .DisposeWith(cleanup);

                this.OneWayBind(ViewModel, v => v.Pot, v => v.PotTextBlock.Text,
                        x => $"Pot : {x}")
                    .DisposeWith(cleanup);

                this.BindCommand(ViewModel, v => v.NextRoundCommand, v => v.NextRoundButton)
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && Enum.TryParse<Play>(button.Content.ToString(), out var play))
            {
                ViewModel.MakePlayCommand.Execute(play).Subscribe();
            }
        }
    }
}
