﻿using System.Reactive.Disposables;
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
            this.WhenActivated(cleanUp =>
            {
                this.OneWayBind(ViewModel, vm => vm.BoardViewModel, v => v.BoardView.ViewModel)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.Spots, v => v.SpotsList.ItemsSource)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.Spots, v => v.TableSpotsList.ItemsSource)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.PlayOptionsViewModel,
                        v => v.PlayOptionsView.ViewModel)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.ChatViewModel, v => v.ChatView.ViewModel)
                    .DisposeWith(cleanUp);
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
