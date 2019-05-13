using System;
using System.Collections.Generic;

namespace PokyPoker.Contracts
{
    public class GameDto
    {
        public List<PlayerDto> Players { get; set; }

        public List<CardDto> Table { get; set; }

        public List<RoundDto> Rounds { get; set; }

        public BettingRulesDto Rules { get; set; }

    }
}
