using System.Windows;
using PokyPoker.Domain;

namespace PokyPoker.Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Card1.Card = new Card(Rank.Ace, Suit.Spades);
            Card2.Card = new Card(Rank.Queen, Suit.Hearts);
            Card3.Card = new Card(Rank.Seven, Suit.Clubs);
        }
    }
}
