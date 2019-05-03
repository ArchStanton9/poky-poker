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
        private readonly Player player;
        private readonly MainViewModel viewModel;

        public PlayerViewModel(Player player, MainViewModel viewModel)
        {
            this.player = player;
            this.viewModel = viewModel;
            Name = player.Name;
            Bank = player.Stack;

            viewModel.WhenAnyValue(v => v.Game, g => g.GetPlayerState(player.Name))
                .ToProperty(this, p => p.RoundState, out roundState);
        }

        [Reactive]
        public string Name { get; set; }

        [Reactive]
        public int Bank { get; set; }
        
        public RoundState RoundState => roundState.Value;
        private readonly ObservableAsPropertyHelper<RoundState> roundState;
    }
}
