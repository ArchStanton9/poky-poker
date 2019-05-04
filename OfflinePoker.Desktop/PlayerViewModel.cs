using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using OfflinePoker.Domain;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace OfflinePoker.Desktop
{
    public class PlayerViewModel : ReactiveObject
    {
        private readonly MainViewModel viewModel;

        public PlayerViewModel(string playerName, MainViewModel viewModel)
        {
            this.viewModel = viewModel;
            Name = playerName;

            viewModel.WhenAnyValue(v => v.Game, g => g.GetPlayerState(playerName))
                .ToProperty(this, p => p.RoundState, out roundState);

            viewModel.WhenAnyValue(v => v.Game, g => g.Players.First(p => p.Name == playerName).Stack)
                .ToProperty(this, p => p.Bank, out bank);
        }

        [Reactive]
        public string Name { get; set; }
        
        public int Bank => bank.Value;
        private readonly ObservableAsPropertyHelper<int> bank;

        public RoundState RoundState => roundState.Value;
        private readonly ObservableAsPropertyHelper<RoundState> roundState;
    }
}
