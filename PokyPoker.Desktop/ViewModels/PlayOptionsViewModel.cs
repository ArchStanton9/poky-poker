using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using PokyPoker.Desktop.Model;
using PokyPoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.ViewModels
{
    public class PlayOptionsViewModel : ReactiveObject
    {
        public PlayOptionsViewModel(GameModel model)
        {
            MakePlayCommand = ReactiveCommand.Create<Play>(p => model.MakeAct(p, Bet));

            model.ObservableGame
                .Select(GetMinBet)
                .ToProperty(this, vm => vm.MinBet, out minBet);

            model.ObservableGame
                .Select(g => g.CurrentPlayer.Stack)
                .ToProperty(this, vm => vm.MaxBet, out maxBet);

            var options = model.ObservableGame
                .Select(g => new HashSet<Play>(g.GetOptions()));

            options
                .Select(o => o.Contains(Play.Bet))
                .ToProperty(this, vm => vm.CanBet, out canBet);

            options
                .Select(o => o.Contains(Play.Raise))
                .ToProperty(this, vm => vm.CanRaise, out canRaise);

            options
                .Select(o => o.Contains(Play.AllIn))
                .ToProperty(this, vm => vm.CanAllIn, out canAllIn);

            options
                .Select(o => o.Contains(Play.Call))
                .ToProperty(this, vm => vm.CanCall, out canCall);

            options
                .Select(o => o.Contains(Play.Check))
                .ToProperty(this, vm => vm.CanCheck, out canCheck);

            options
                .Select(o => o.Contains(Play.Fold))
                .ToProperty(this, vm => vm.CanFold, out canFold);
        }

        private static int GetMinBet(Game game)
        {
            var maxBet = game.CurrentRound.MaxBet;
            if (maxBet == 0)
                return game.Rules.BigBlind;

            var playerBet = game.GetPlayerState(game.CurrentPlayer).Bet;
            return maxBet - playerBet;
        }

        public ReactiveCommand<Play, Unit> MakePlayCommand;

        [Reactive]
        public int Bet { get; set; }

        private readonly ObservableAsPropertyHelper<int> minBet;
        public int MinBet => minBet.Value;

        private readonly ObservableAsPropertyHelper<int> maxBet;
        public int MaxBet => maxBet.Value;

        private readonly ObservableAsPropertyHelper<bool> canBet;
        public bool CanBet => canBet.Value;

        private readonly ObservableAsPropertyHelper<bool> canRaise;
        public bool CanRaise => canRaise.Value;

        private readonly ObservableAsPropertyHelper<bool> canCall;
        public bool CanCall => canCall.Value;

        private readonly ObservableAsPropertyHelper<bool> canAllIn;
        public bool CanAllIn => canAllIn.Value;

        private readonly ObservableAsPropertyHelper<bool> canCheck;
        public bool CanCheck => canCheck.Value;

        private readonly ObservableAsPropertyHelper<bool> canFold;
        public bool CanFold => canFold.Value;
    }
}
