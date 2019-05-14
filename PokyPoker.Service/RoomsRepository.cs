using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public class RoomsRepository : IRoomsRepository
    {
        private readonly ConcurrentDictionary<Guid, Room> dictionary = new ConcurrentDictionary<Guid, Room>();

        public RoomsRepository()
        {
            AddRoom(new Room()
            {
                RoomId = Guid.Parse("2F12F01A-7BE2-45BF-B995-82F4137C3EAF"),
                Members = new List<RoomMember>(),
                Rules = new BettingRules()
                {
                    BigBlind = 40,
                    SmallBlind = 20
                }
            });
        }

        public Task AddRoom(Room room)
        {
            if (room.RoomId == Guid.Empty)
                room.RoomId = Guid.NewGuid();

            var result = dictionary.TryAdd(room.RoomId, room);
            return Task.FromResult(result);
        }

        public Task AddRoomMember(Guid roomId, RoomMember member)
        {
            if (dictionary.TryGetValue(roomId, out var room))
            {
                room.Members.Add(member);
            }

            return Task.FromResult(false);
        }

        public Task<Room> GetRoom(Guid roomId)
        {
            return Task.FromResult(dictionary[roomId]);
        }

        public Task<IEnumerable<Room>> GetAll()
        {
            return Task.FromResult((IEnumerable<Room>) dictionary.Values);
        }
    }
}
