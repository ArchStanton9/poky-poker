using PokyPoker.Contracts;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public interface IGameMapper
    {
        GameDto Map(Game game);

        Game Map(GameDto gameDto);
    }
}
