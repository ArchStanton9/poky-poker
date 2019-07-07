using System.Reactive;
using System.Threading.Tasks;
using PokyPoker.Client;
using PokyPoker.Contracts;
using PokyPoker.Contracts.Requests;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PokyPoker.Desktop.Dealer
{
    public class RoomBuilderViewModel : ReactiveObject
    {
        private readonly IPokerClient client;

        public RoomBuilderViewModel(IPokerClient client)
        {
            this.client = client;
            CreateRoomCommand = ReactiveCommand.CreateFromTask(CreateRoom);
        }

        private async Task CreateRoom()
        {
            var request = new CreateRoomRequest();
            request.PlayersCount = PlayersCount;
            request.Rules = new BettingRulesDto
            {
                BigBlind = BigBlind,
                SmallBlind = SmallBlind
            };

            var result = await client.CreateRoom(request);
            if (!result.IsSuccessful)
            {
                ErrorMessage = $"Failed to create room. StatusCode: {result.StatusCode}";
            }
        }

        [Reactive]
        public int BigBlind { get; set; }

        [Reactive]
        public int SmallBlind { get; set; }

        [Reactive]
        public int PlayersCount { get; set; }

        [Reactive]
        public string ErrorMessage { get; set; }

        public ReactiveCommand<Unit, Unit> CreateRoomCommand { get; }
    }
}
