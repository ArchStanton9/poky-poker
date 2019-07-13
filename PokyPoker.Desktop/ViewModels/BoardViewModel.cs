using System.Collections.ObjectModel;
using DynamicData.Binding;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop.ViewModels
{
    public class BoardViewModel : ReactiveObject
    {
        public ObservableCollection<Card> BoardCards { get; set; } = new ObservableCollectionExtended<Card>()
        {
            new Card(Rank.Ace, Suit.Spades),
            new Card(Rank.Queen, Suit.Hearts),
            new Card(Rank.King, Suit.Hearts)
        };
    }
}
