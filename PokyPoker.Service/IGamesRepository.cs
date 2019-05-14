using System;
using System.Threading.Tasks;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public interface IGamesRepository
    {
        Task<Game> GetGameAsync(Guid gameId);

        Task<Game> SetGameAsync(Guid gameId, Game game);
    }
}
