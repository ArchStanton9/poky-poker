using System;
using System.Reactive.Linq;
using PokyPoker.Domain;
using ReactiveUI;

namespace PokyPoker.Desktop
{
    public class HandViewModel : ReactiveObject
    {
        public HandViewModel(IObservable<Hand> observableHand)
        {
            observableHand
                .Select(h => h.ToString())
                .ToProperty(this, v => v.Name, out name);

            observableHand
                .Select(HandFormatter.Format)
                .ToProperty(this, v => v.Cards, out cards);
        }

        public string Name => name.Value;
        private readonly ObservableAsPropertyHelper<string> name;

        public string Cards => cards.Value;
        private readonly ObservableAsPropertyHelper<string> cards;
    }
}
