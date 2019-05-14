using System;

namespace PokyPoker.Contracts
{
    public class PlayerInfoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Chips { get; set; }
    }
}
