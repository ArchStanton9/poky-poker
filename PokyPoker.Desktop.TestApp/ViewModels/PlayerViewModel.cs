using System;
using System.Linq;
using System.Reactive.Linq;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.TestApp.ViewModels
{
    public class PlayerViewModel : ReactiveObject
    {
        public PlayerViewModel(int playerSpot, IObservable<Game> observableGame)
        {
            Name = $"p{playerSpot}";
            observableGame
                .Select(s => s.GetPlayerState(playerSpot))
                .ToProperty(this, p => p.RoundState, out roundState);

            observableGame
                .Select(g => g.Players.First(p => p.Id == playerSpot).Stack)
                .ToProperty(this, p => p.Bank, out bank);

            Hand = new HandViewModel(
                observableGame.Select(g => g.Players.First(p => p.Id == playerSpot).Hand),
                observableGame.Select(g => g.CurrentTable)
            );
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
