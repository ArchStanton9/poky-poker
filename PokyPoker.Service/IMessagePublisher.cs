using System.Threading.Tasks;

namespace PokyPoker.Service
{
    public interface IMessagePublisher
    {
        Task PublishAsync<TMessage>(TMessage message);
    }
}
