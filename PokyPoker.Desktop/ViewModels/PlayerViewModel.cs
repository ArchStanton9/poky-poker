using System;
using System.Linq;
using System.Reactive.Linq;
using PokyPoker.Desktop.Model;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class PlayerViewModel : ReactiveObject
    {
        private readonly PlayerModel model;

        public PlayerViewModel(PlayerModel model, IObservable<Game> game)
        {
            this.model = model;

            var player = game.Select(g => g.Players.First(p => p.Spot == model.Spot));
            game
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(g => g.GetPlayerState(model.Player))
                .ToProperty(this, vm => vm.PlayerState, out playerState);

            player
                .Select(p => p.Hand.First())
                .ToProperty(this, vm => vm.LeftCard, out leftCard);

            player
                .Select(p => p.Hand.Last())
                .ToProperty(this, vm => vm.RightCard, out rightCard);

            player
                .Select(p => p.Stack)
                .ToProperty(this, vm => vm.Currency, out currency);
        }

        public int Spot => model.Spot;

        public string Name => model.Name;

        public string PhotoUrl => model.PhotoUrl;

        private readonly ObservableAsPropertyHelper<PlayerState> playerState;
        public PlayerState PlayerState => playerState.Value;

        private readonly ObservableAsPropertyHelper<int> currency;
        public int Currency => currency.Value;

        private readonly ObservableAsPropertyHelper<Card> leftCard;
        public Card LeftCard => leftCard.Value;

        private readonly ObservableAsPropertyHelper<Card> rightCard;
        public Card RightCard => rightCard.Value;
    }
}
