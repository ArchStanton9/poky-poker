using System.Threading.Tasks;
using PokyPoker.Contracts.Requests;

namespace PokyPoker.Client
{
    public interface IPokerClient
    {
        Task<ApiResponse> CreateRoom(CreateRoomRequest createRoomRequest);
    }
}
