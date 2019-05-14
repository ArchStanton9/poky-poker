using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PokyPoker.Contracts;
using PokyPoker.Contracts.Requests;
using PokyPoker.Domain;
using PokyPoker.Service;

namespace PokyPoker.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsRepository repository;

        public RoomsController(IRoomsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Room>>> Get()
        {
            var room = await repository.GetAll();
            return room.ToArray();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(Guid id)
        {
            var room = await repository.GetRoom(id);
            return room;
        }


        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Guid>> CreateNew([FromBody] CreateRoomRequest requestBody)
        {
            var room = new Room();
            room.RoomId = Guid.NewGuid();
            room.Members = new List<RoomMember>();
            room.Rules = new BettingRules
            {
                BigBlind = requestBody.Rules.BigBlind,
                SmallBlind = requestBody.Rules.SmallBlind,
            };

            await repository.AddRoom(room);

            return room.RoomId;
        }


        [HttpPost]
        [Route("/members/{id}")]
        public async Task<ActionResult> Join(Guid id, [FromQuery] int spot, [FromBody] PlayerInfoDto playerInfo)
        {
            var member = new RoomMember();
            member.Spot = spot;
            member.Chips = playerInfo.Chips;
            member.Name = playerInfo.Name;
            member.UserId = playerInfo.Id;

            await repository.AddRoomMember(id, member);

            return NoContent();
        }
    }
}
