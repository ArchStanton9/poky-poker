using System.Collections.Generic;

namespace PokyPoker.Contracts
{
    public class PlayerDto
    {
        public int Id { get; set; }

        public List<CardDto> Hand { get; set; }

        public bool IsActive { get; set; }

        public int Stack { get; set; }
    }
}
