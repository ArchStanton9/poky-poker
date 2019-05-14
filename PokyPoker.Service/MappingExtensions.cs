using PokyPoker.Contracts;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public static class MappingExtensions
    {
        public static GameDto AsDto(this Game game, IGameMapper mapper) => mapper.Map(game);

        public static Game AsDomainGame(this GameDto dto, IGameMapper mapper) => mapper.Map(dto);
    }
}
