using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.ViewModels
{
    public class ChatViewModel : ReactiveObject
    {
        public ChatViewModel(IReadOnlyCollection<PlayerViewModel> players)
        {
            var messagesSource = new SourceList<string>();
            messagesSource
                .Connect()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out messages)
                .Subscribe();

            SendMessageCommand = ReactiveCommand.Create<string>(msg =>
            {
                var player = players.SingleOrDefault(p => p.PlayerState.ShouldAct);
                messagesSource.Add($"{player?.Name ?? "Anonymous"}: {msg}");
            });
        }

        private readonly ReadOnlyObservableCollection<string> messages;
        public ReadOnlyObservableCollection<string> Messages => messages;

        [Reactive]
        public string CurrentMessage { get; set; }

        public ReactiveCommand<string, Unit> SendMessageCommand { get; }
    }
}
