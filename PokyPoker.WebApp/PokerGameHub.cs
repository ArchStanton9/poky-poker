using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PokyPoker.WebApp
{
    public class PokerGameHub : Hub
    {
        public Task Ping()
        {
            return Clients.Caller.SendAsync("pong");
        }
    }
}
