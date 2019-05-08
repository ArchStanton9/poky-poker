using System;
using System.Linq;
using System.Reactive.Linq;
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

            Hand = new HandViewModel(observableGame.Select(GetHand), observableGame.Select(GetTable));
        }

        public Hand GetHand(Game game)
        {
            var player = game.Players.First(p => p.Name == Name);
            return player.Hand;    
        }

        public static Card[] GetTable(Game game)
        {
            switch (game.Stage)
            {
                case Stage.PreFlop:
                    return new Card[0];
                case Stage.Flop:
                    return game.Table.Take(3).ToArray();
                case Stage.Turn:
                    return game.Table.Take(4).ToArray();
                case Stage.River:
                    return game.Table.Take(5).ToArray();
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
