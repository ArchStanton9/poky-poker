using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        public MainViewModel()
        {
            Player1 = new PlayerViewModel()
            {
                Name = "Johnny \"The Fox\"",
                Currency = 400,
                LastPlay = Play.Call,
                PhotoUrl = "Assets/Avatar1.png",
                LeftCard = new Card(Rank.Ace, Suit.Spades),
                RightCard = new Card(Rank.Queen, Suit.Hearts)
            };

            Player2 = new PlayerViewModel()
            {
                Name = "Poker Queen",
                Currency = 500,
                LastPlay = Play.Call,
                PhotoUrl = "Assets/Avatar2.png",
                LeftCard = new Card(Rank.Jack, Suit.Clubs),
                RightCard = new Card(Rank.Jack, Suit.Diamonds)
            };

            Player3 = new PlayerViewModel()
            {
                Name = "Gambler #3",
                Currency = 600,
                LastPlay = Play.Call,
                PhotoUrl = "Assets/Avatar3.png",
                LeftCard = new Card(Rank.Five, Suit.Diamonds),
                RightCard = new Card(Rank.Seven, Suit.Clubs)
            };

            Player4 = new PlayerViewModel()
            {
                Name = "Gandalf the grey",
                Currency = 1000,
                LastPlay = Play.Call,
                PhotoUrl = "Assets/Avatar4.png",
                LeftCard = new Card(Rank.King, Suit.Diamonds),
                RightCard = new Card(Rank.King, Suit.Clubs)
            };

            PlayOptionsViewModel = new PlayOptionsViewModel();
        }

        public BoardViewModel BoardViewModel { get; set; } = new BoardViewModel();

        public PlayerViewModel Player1 { get; set; }

        public PlayerViewModel Player2 { get; set; }

        public PlayerViewModel Player3 { get; set; }

        public PlayerViewModel Player4 { get; set; }

        public PlayOptionsViewModel PlayOptionsViewModel { get; set; }
    }
}
