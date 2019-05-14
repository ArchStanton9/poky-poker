using System;
using System.Collections.Generic;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public class Room
    {
        public Guid RoomId { get; set; }

        public BettingRules Rules { get; set; }

        public List<RoomMember> Members { get; set; }
    }
}
