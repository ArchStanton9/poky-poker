using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
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
                new Player("p1", deck.Take(2), true, 4000),
                new Player("p2", deck.Take(2), true, 5000),
                new Player("p3", deck.Take(2), true, 2000),
            };

            var gameObservable = this.WhenAnyValue(v => v.Game);

            Game = Game.StartNew(BettingRules.Standard, players, deck.Take(5));

            Table = new TableViewModel(gameObservable);
            Players = new ObservableCollection<PlayerViewModel>(
                players.Select(p => new PlayerViewModel(p.Name, gameObservable)));

            PlayOptionsViewModel = new PlayOptionsViewModel(
                gameObservable,
                (play, bet) => Game = Game.MakeAct(play, bet));

            NextRoundCommand = ReactiveCommand.Create<Unit>(x => OnNextRound());
        }

        private void OnNextRound()
        {
            if (Game.IsComplete)
            {
                var result = Game.GetResult();
                var players = new Queue<Player>(result);
                var player = players.Dequeue();
                players.Enqueue(player);

                Game = Game.StartNew(BettingRules.Standard, players.ToArray(), Deck.BuildStandard());
                return;
            }

            Game = Game.NextRound();
        }

        [Reactive]
        public Game Game { get; set; }
        public ObservableCollection<PlayerViewModel> Players { get; }
        public ReactiveCommand<Unit, Unit> NextRoundCommand { get; set; }

        [Reactive]
        public TableViewModel Table { get; set; }

        [Reactive]
        public PlayOptionsViewModel PlayOptionsViewModel { get; set; }
    }
}
