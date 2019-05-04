using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using OfflinePoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OfflinePoker.Desktop
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            var deck = Deck.CreateStandardDeck();
            var players = new[]
            {
                new Player("p1", deck.Take(2), true, 4000),
                new Player("p2", deck.Take(2), true, 5000),
                new Player("p3", deck.Take(2), true, 2000),
            };

            Game = Game.StartNew(BettingRules.Standard, players, deck.Take(5));
            Players = new ObservableCollection<PlayerViewModel>(
                players.Select(p => new PlayerViewModel(p.Name, this.WhenAnyValue(v => v.Game))));

            PlayOptionsViewModel = new PlayOptionsViewModel(
                this.WhenAnyValue(v => v.Game),
                (play, bet) => Game = Game.MakeAct(play, bet));

            NextRoundCommand = ReactiveCommand
                .Create<Play>(p => Game = Game.NextRound());
            
            this.WhenAnyValue(v => v.Game, (Game g) => g.Pot)
                .ToProperty(this, v => v.Pot, out pot);
        }
        
        [Reactive]
        public Game Game { get; set; }

        [Reactive]
        public string CurrentPlayer { get; set; }

        public int Pot => pot.Value;
        private readonly ObservableAsPropertyHelper<int> pot;

        public ObservableCollection<PlayerViewModel> Players { get; }
        public ReactiveCommand<Play, Unit> NextRoundCommand { get; set; }

        [Reactive]
        public PlayOptionsViewModel PlayOptionsViewModel { get; set; }
    }
}
