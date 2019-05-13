using System.Reactive;
using System.Reactive.Linq;
using PokyPoker.Desktop.Models;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class CallViewModel : ReactiveObject, IPlayViewModel
    {
        public CallViewModel(GameModel model)
        {
            model.ObservableGame.Select(FormDisplayName)
                .ToProperty(this, vm => vm.DisplayName, out displayName);

            ActCommand = ReactiveCommand.Create(() =>
            {
                // todo refactor

                var game = model.Game;
                var maxBet = game.CurrentRound.MaxBet;
                var playerBet = game.GetPlayerState(game.CurrentPlayer.Id).Bet;

                model.MakeAct(Play.Call, maxBet - playerBet);
            });
        }

        private static string FormDisplayName(Game game)
        {
            var maxBet = game.CurrentRound.MaxBet;
            var playerBet = game.GetPlayerState(game.CurrentPlayer.Id).Bet;

            return $"Call ({maxBet - playerBet})";
        }

        public Play Play { get; } = Play.Call;

        public string DisplayName => displayName.Value;
        private readonly ObservableAsPropertyHelper<string> displayName;

        public ReactiveCommand<Unit, Unit> ActCommand { get; }
    }
}
