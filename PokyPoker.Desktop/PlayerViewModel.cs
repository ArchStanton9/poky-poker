using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop
{
    public class PlayerViewModel : ReactiveObject
    {
        public PlayerViewModel(string playerName, IObservable<Game> observableGame)
        {
            Name = playerName;
            observableGame
                .Select(s => s.GetPlayerState(playerName))
                .ToProperty(this, p => p.RoundState, out roundState);

            observableGame
                .Select(g => g.Players.First(p => p.Name == playerName).Stack)
                .ToProperty(this, p => p.Bank, out bank);

            var hand = observableGame.Select(g => g.Players.First(p => p.Name == playerName).Hand);
            Hand = new HandViewModel(hand);
        }

        [Reactive]
        public string Name { get; set; }

        public int Bank => bank.Value;
        private readonly ObservableAsPropertyHelper<int> bank;

        public RoundState RoundState => roundState.Value;
        private readonly ObservableAsPropertyHelper<RoundState> roundState;

        public HandViewModel Hand { get; }
    }
}
