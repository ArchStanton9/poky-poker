using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.Model
{
    public class GameModel : ReactiveObject
    {
        public GameModel()
        {
            playersSource = new SourceList<PlayerModel>();
            playersSource.Add(new PlayerModel
            {
                Spot = 0,
                Name = "Johnny \"The Fox\"",
                Stack = 5000,
                PhotoUrl = "Assets/Avatar1.png"
            });

            playersSource.Add(new PlayerModel
            {
                Spot = 1,
                Name = "Poker Queen",
                Stack = 4500,
                PhotoUrl = "Assets/Avatar2.png"
            });

            playersSource.Add(new PlayerModel
            {
                Spot = 2,
                Name = "Gambler #3",
                Stack = 6000,
                PhotoUrl = "Assets/Avatar3.png"
            });

            playersSource.Add(new PlayerModel
            {
                Spot = 3,
                Name = "Gandalf The Grey",
                Stack = 8000,
                PhotoUrl = "Assets/Avatar4.png"
            });

            var deck = Deck.BuildStandard();
            var players = playersSource.Items
                .Select(p => new Player(p.Spot, deck.Take(2), true, p.Stack))
                .ToArray();

            Game = Game.StartNew(BettingRules.Standard, players, deck.Take(5));
            ObservableGame = this.WhenAnyValue(m => m.Game);
        }

        private readonly SourceList<PlayerModel> playersSource;
        public IObservable<IChangeSet<PlayerModel>> Players => playersSource.Connect();


        [Reactive]
        public Game Game { get; private set; }

        public IObservable<Game> ObservableGame { get; }

        
        public void MakeAct(Play play, int bet = 0)
        {
            Game = MakeAct(Game, play, bet);
        }

        private static Game MakeAct(Game game, Play play, int bet = 0)
        {
            game = game.MakeAct(play, bet);

            if (game.IsComplete)
            {
                var result = game.GetResult();
                var players = new Queue<Player>(result);
                var player = players.Dequeue();
                players.Enqueue(player);

                game = Game.StartNew(BettingRules.Standard, players.ToArray(), Deck.BuildStandard());
            }

            if (game.CurrentRound.IsComplete)
            {
                game = game.NextRound();
            }

            return game;
        }
    }
}
