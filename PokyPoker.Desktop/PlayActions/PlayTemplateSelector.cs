using System;
using System.Windows;
using System.Windows.Controls;

namespace PokyPoker.Desktop.PlayActions
{
    public class PlayTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PlayTemplate { get; set; }

        public DataTemplate PlayWithBetTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IPlayWithBet)
                return PlayWithBetTemplate;

            if (item is IPlayViewModel)
                return PlayTemplate;

            throw new ArgumentException($"Item is supposed to implement {nameof(IPlayViewModel)}");
        }
    }
}
