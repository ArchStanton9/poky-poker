using PokyPoker.Domain;

namespace PokyPoker.Desktop.Model
{
    public class PlayerModel
    {
        public PlayerModel(Player player)
        {
            Player = player;
        }

        public Player Player { get; }

        public int Spot => Player.Spot;

        public int Stack => Player.Stack;

        public string Name { get; set; }

        public string PhotoUrl { get; set; }
    }
}
