using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PokyPoker.Contracts;
using PokyPoker.Domain;

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

            Games.GetOrAdd("42", mapper.Map(game));
        }

        private static readonly DtoMapper mapper = new DtoMapper();

        private static readonly ConcurrentDictionary<string, GameDto> Games =
            new ConcurrentDictionary<string, GameDto>();

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return Games.Keys.ToArray();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<GameDto> Get(string id)
        {
            if (Games.TryGetValue(id, out var dto))
                return Ok(dto);

            return NotFound();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public ActionResult<GameDto> Put(string id, [FromBody] ActDto act)
        {
            if (!Games.TryGetValue(id, out var value))
                return NotFound();

            var game = mapper.Map(value);
            game = game.MakeAct((Play) act.Play, act.Bet);

            if (game.IsComplete)
            {
                var result = game.GetResult();
                var players = new Queue<Player>(result);
                var player = players.Dequeue();
                players.Enqueue(player);

                game = Game.StartNew(BettingRules.Standard, players.ToArray(), Deck.BuildStandard());
            }

            if (game.CurrentRound.IsComplete)
            {
                game = game.NextRound();
            }

            var dto = mapper.Map(game);
            Games[id] = dto;

            return Ok(dto);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
