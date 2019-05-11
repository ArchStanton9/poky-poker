using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var deck = Deck.BuildStandard();
            var players = new[]
            {
                new Player(0, deck.Take(2), true, 4000),
                new Player(1, deck.Take(2), true, 5000),
                new Player(2, deck.Take(2), true, 2000),
            };

            Game = Game.StartNew(BettingRules.Standard, players, deck.Take(5));

            var gameObservable = this.WhenAnyValue(v => v.Game);
            Players = new ObservableCollection<PlayerViewModel>(
                players.Select(p => new PlayerViewModel(p.Id, gameObservable)));

            Table = new TableViewModel(gameObservable);
            PlayOptionsViewModel = new PlayOptionsViewModel(gameObservable, OnAct);
        }

        private void OnAct(Play play, int bet) => Game = OnAct(Game, play, bet);

        private static Game OnAct(Game game, Play play, int bet)
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

        [Reactive]
        public Game Game { get; set; }

        public ObservableCollection<PlayerViewModel> Players { get; }

        [Reactive]
        public TableViewModel Table { get; set; }

        [Reactive]
        public PlayOptionsViewModel PlayOptionsViewModel { get; set; }
    }
}
