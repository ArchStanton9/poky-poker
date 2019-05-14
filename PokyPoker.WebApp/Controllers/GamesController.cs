using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PokyPoker.Contracts;
using PokyPoker.Domain;
using PokyPoker.Service;

namespace PokyPoker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        static GamesController()
        {
            var deck = Deck.BuildStandard();

            var p1 = new Player(0, deck.Take(2), true, 5000);
            var p2 = new Player(1, deck.Take(2), true, 4000);
            var p3 = new Player(2, deck.Take(2), true, 6000);

            var rules = new BettingRules
            {
                BigBlind = 40,
                SmallBlind = 20
            };

            var players = new[] {p1, p2, p3};
            var table = deck.Take(5);

            var game = Game.StartNew(rules, players, table);

            repository.SetGameAsync(Guid.Parse("5a827b17-87d9-4226-9af6-bf567e99b0f1"), game);
        }

        private static readonly MemoryGameRepository repository = new MemoryGameRepository();
        private static readonly DtoMapper mapper = new DtoMapper();


        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return repository.GameIds.Select(id => id.ToString()).ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> Get(Guid id)
        {
            var game = await repository.GetGameAsync(id);
            return game.AsDto(mapper);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<ActionResult<GameDto>> Put(Guid id, [FromBody] ActDto act)
        {
            var command = new MakeActCommand
            {
                GameId = id,
                Player = act.Player,
                Play = (Play) act.Play,
                Bet = act.Bet,
            };

            var handler = new MakeActHandler(repository);
            var game = await handler.Handle(command);

            return game.AsDto(mapper);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
