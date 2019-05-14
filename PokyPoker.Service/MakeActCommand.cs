using System;
using PokyPoker.Domain;

namespace PokyPoker.Service
{
    public class MakeActCommand
    {
        public Guid GameId { get; set; }


        public int Player { get; set; }

        public Play Play { get; set; }

        public int Bet { get; set; }
    }
}
