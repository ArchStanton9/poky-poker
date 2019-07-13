using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.ViewModels
{
    public class ChatViewModel : ReactiveObject
    {
        public ChatViewModel()
        {
            var messagesSource = new SourceList<string>();
            messagesSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out messages)
                .Subscribe();

            SendMessageCommand = ReactiveCommand.Create<string>(msg =>
            {
                messagesSource.Add($"Anonymous: {msg}");
            });
        }

        public ReadOnlyObservableCollection<string> Messages => messages;
        private readonly ReadOnlyObservableCollection<string> messages;

        [Reactive]
        public string CurrentMessage { get; set; }

        public ReactiveCommand<string, Unit> SendMessageCommand { get; }
    }
}
