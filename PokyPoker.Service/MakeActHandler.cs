using System.Threading.Tasks;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public class MakeActHandler
    {
        private readonly IGamesRepository repository;

        public MakeActHandler(IGamesRepository repository)
        {
            this.repository = repository;
        }

        public async Task<Game> Handle(MakeActCommand command)
        {
            var game = await repository.GetGameAsync(command.GameId);

            var player = game.Players[command.Player];
            game = game.MakeAct(player, command.Play, command.Bet);

            if (game.CurrentRound.IsComplete)
            {
                game = game.NextRound();
            }

            await repository.SetGameAsync(command.GameId, game);

            return game;
        }
    }
}
