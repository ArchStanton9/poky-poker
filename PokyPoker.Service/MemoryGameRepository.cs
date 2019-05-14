using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public class MemoryGameRepository : IGamesRepository
    {
        private readonly ConcurrentDictionary<Guid, Game> dictionary = new ConcurrentDictionary<Guid, Game>();

        public Task<Game> GetGameAsync(Guid gameId)
        {
            if (dictionary.TryGetValue(gameId, out var value))
            {
                return Task.FromResult(value);
            }

            throw new Exception("Get not found");
        }

        public Task<Game> SetGameAsync(Guid gameId, Game game)
        {
            var result = dictionary.AddOrUpdate(gameId, game, (i, g) => game);
            return Task.FromResult(result);
        }

        public Guid[] GameIds => dictionary.Keys.ToArray();
    }
}
