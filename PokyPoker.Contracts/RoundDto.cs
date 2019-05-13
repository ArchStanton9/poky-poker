using System;
using System.Collections.Generic;
using System.Text;

namespace PokyPoker.Contracts
{
    public class RoundDto
    {
        public int PlayersCount { get; set; }

        public List<ActDto> Acts { get; set; }
    }
}
