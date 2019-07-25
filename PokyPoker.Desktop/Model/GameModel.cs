using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
            var deck = Deck.BuildStandard();
            playersSource = new SourceList<PlayerModel>();
            playersSource.Add(new PlayerModel(new Player(0, deck.Take(2), true, 5000))
            {
                Name = "Johnny \"The Fox\"",
                PhotoUrl = "Assets/Avatar1.png"
            });

            playersSource.Add(new PlayerModel(new Player(1, deck.Take(2), true, 4500))
            {
                Name = "Poker Queen",
                PhotoUrl = "Assets/Avatar2.png"
            });

            playersSource.Add(new PlayerModel(new Player(2, deck.Take(2), true, 6000))
            {
                Name = "Gambler #3",
                PhotoUrl = "Assets/Avatar3.png"
            });

            playersSource.Add(new PlayerModel(new Player(3, deck.Take(2), true, 8000))
            {
                Name = "Gandalf The Grey",
                PhotoUrl = "Assets/Avatar4.png"
            });

            Game = Game.StartNew(BettingRules.Standard, playersSource.Items.Select(i => i.Player).ToArray(),
                deck.Take(5));
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

            return game;
        }
    }
}
