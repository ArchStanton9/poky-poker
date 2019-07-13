using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class PlayerViewModel : ReactiveObject
    {
        public PlayerViewModel()
        {
            
        }


        public string Name { get; set; }

        public int Currency { get; set; }

        public Play LastPlay { get; set; }

        public string PhotoUrl { get; set; }

        public Card LeftCard { get; set; }

        public Card RightCard { get; set; }
    }
}
