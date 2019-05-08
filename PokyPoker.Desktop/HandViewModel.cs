using System;
using System.Reactive.Linq;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop
{
    public class HandViewModel : ReactiveObject
    {
        public HandViewModel(IObservable<Hand> observableHand, IObservable<Card[]> observableTable)
        {
            observableHand
                .Select(HandFormatter.Format)
                .ToProperty(this, v => v.Cards, out cards);

            observableTable
                .Select(GetHandName)
                .ToProperty(this, v => v.Name, out name);
        }

        public string GetHandName(Card[] tableCards)
        {
            var hand = CardsParser.ParseHand(Cards);
            if (tableCards.Length > 0)
            {
                hand = hand.Combine(tableCards);
            }

            return hand.Name.ToString();
        }

        public string Name => name.Value;
        private readonly ObservableAsPropertyHelper<string> name;

        public string Cards => cards.Value;
        private readonly ObservableAsPropertyHelper<string> cards;
    }
}
