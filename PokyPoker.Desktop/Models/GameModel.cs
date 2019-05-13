using System;
using System.Collections.Generic;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.Models
{
    public class GameModel : ReactiveObject
    {
        public GameModel()
        {
            var deck = Deck.BuildStandard();
            Players = new[]
            {
                new Player(0, deck.Take(2), true, 4000),
                new Player(1, deck.Take(2), true, 5000),
                new Player(2, deck.Take(2), true, 2000),
            };

            Game = Game.StartNew(BettingRules.Standard, Players, deck.Take(5));
            ObservableGame = this.WhenAnyValue(m => m.Game);
        }

        [Reactive]
        public Game Game { get; private set; }

        public Player[] Players { get; }

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
