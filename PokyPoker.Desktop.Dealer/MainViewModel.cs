using System;
using PokyPoker.Client;

namespace PokyPoker.Desktop.Dealer
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            var url = new Uri("http://localhost:8080");
            var client = new PokyPokerClient(url);

            RoomBuilder = new RoomBuilderViewModel(client);
        }

        public RoomBuilderViewModel RoomBuilder { get; set; }
    }
}
