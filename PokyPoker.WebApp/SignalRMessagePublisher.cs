using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PokyPoker.Service;

namespace PokyPoker.WebApp
{
    public class SignalRMessagePublisher : IMessagePublisher
    {
        private readonly IHubContext<PokerGameHub> context;

        public SignalRMessagePublisher(IHubContext<PokerGameHub> context)
        {
            this.context = context;
        }

        public Task PublishAsync<TMessage>(TMessage message)
        {
            return context.Clients.All.SendAsync("message", message);
        }
    }
}
