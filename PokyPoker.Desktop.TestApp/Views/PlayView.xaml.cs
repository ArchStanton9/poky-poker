﻿using System.Reactive.Disposables;
using PokyPoker.Desktop.TestApp.ViewModels;
using ReactiveUI;

namespace PokyPoker.Desktop.TestApp.Views
{
    /// <summary>
    /// Interaction logic for PlayView.xaml
    /// </summary>
    public partial class PlayView : ReactiveUserControl<IPlayViewModel>
    {
        public PlayView()
        {
            InitializeComponent();

            this.WhenActivated(cleanUp =>
            {
                this.BindCommand(ViewModel, vm => vm.ActCommand, v => v.ActButton)
                    .DisposeWith(cleanUp);

                this.OneWayBind(ViewModel, vm => vm.DisplayName, v => v.ActButton.Content)
                    .DisposeWith(cleanUp);
            });
        }
    }
}