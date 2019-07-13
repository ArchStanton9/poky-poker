using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class PlayerViewModel : ReactiveObject
    {
        public PlayerViewModel()
        {
            Name = "Johnny \"The Fox\"";
            Currency = 400;
            LastPlay = Play.Call;
            PhotoUrl = "Assets/Avatar1.png";
            LeftCard = new Card(Rank.Ace, Suit.Spades);
            RightCard = new Card(Rank.Queen, Suit.Hearts);
        }


        public string Name { get; set; }

        public int Currency { get; set; }

        public Play LastPlay { get; set; }

        public string PhotoUrl { get; set; }

        public Card LeftCard { get; set; }

        public Card RightCard { get; set; }
    }
}
