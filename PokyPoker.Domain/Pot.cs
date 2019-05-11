using System.Collections.Generic;

namespace PokyPoker.Domain
{
    public class Pot
    {
        private readonly HashSet<Player> contenders;

        public Pot(int amount, HashSet<Player> contenders)
        {
            this.contenders = contenders;
            Amount = amount;
        }

        public int Amount { get; }

        public IEnumerable<Player> Contenders => contenders;

        public bool IsContender(Player player) => contenders.Contains(player);

        public static Pot Create(IEnumerable<Player> contenders, int amount)
            => new Pot(amount, new HashSet<Player>(contenders));

        public static implicit operator int(Pot pot) => pot.Amount;
    }
}
