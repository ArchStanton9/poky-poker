using System;

namespace PokyPoker.Service
{
    public class RoomMember
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public int Spot { get; set; }

        public int Chips { get; set; }
    }
}
