using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokyPoker.Service
{
    public interface IRoomsRepository
    {
        Task AddRoom(Room room);
        Task AddRoomMember(Guid roomId, RoomMember member);

        Task<Room> GetRoom(Guid roomId);

        Task<IEnumerable<Room>> GetAll();
    }
}