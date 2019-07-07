using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PokyPoker.Domain;

namespace PokyPoker.Desktop.Controls
{
    public class CardView : Control
    {
        static CardView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CardView), new FrameworkPropertyMetadata(typeof(CardView)));
        }

        public static readonly DependencyProperty CardProperty = DependencyProperty.Register(
            "Card", typeof(Card), typeof(CardView), new PropertyMetadata(default(Card), OnCardPropertyChangedCallback));

        public Card Card
        {
            get => (Card) GetValue(CardProperty);
            set => SetValue(CardProperty, value);
        }

        private static void OnCardPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is Card card && d is CardView cardView)
                cardView.OnCardChanged(card);
        }


        public static readonly DependencyProperty FrontSideSourceProperty = DependencyProperty.Register(
            "FrontSideSource", typeof(ImageSource), typeof(CardView), new PropertyMetadata(default(ImageSource)));

        public ImageSource FrontSideSource
        {
            get => (ImageSource) GetValue(FrontSideSourceProperty);
            set => SetValue(FrontSideSourceProperty, value);
        }


        public static readonly DependencyProperty BackSideSourceProperty = DependencyProperty.Register(
            "BackSideSource", typeof(ImageSource), typeof(CardView), new PropertyMetadata(default(ImageSource)));

        public ImageSource BackSideSource
        {
            get => (ImageSource) GetValue(BackSideSourceProperty);
            set => SetValue(BackSideSourceProperty, value);
        }


        public static readonly DependencyProperty CardsSourceProperty = DependencyProperty.Register(
            "CardsSource", typeof(BitmapSource), typeof(CardView), new PropertyMetadata(default(BitmapSource)));

        public BitmapSource CardsSource
        {
            get => (BitmapSource) GetValue(CardsSourceProperty);
            set => SetValue(CardsSourceProperty, value);
        }

        private void OnCardChanged(Card card)
        {
            var key = (int) card.Suit << 8 + (int) card.Rank;
            if (!cardsFrontSources.TryGetValue(key, out var source))
            {
                var rect = GetCardRect(card);
                source = new CroppedBitmap(CardsSource, rect);
                cardsFrontSources[key] = source;
            }

            FrontSideSource = source;
        }

        private static readonly Rank[] ranksOrder =
        {
            Rank.Ace,
            Rank.Two,
            Rank.Three,
            Rank.Four,
            Rank.Five,
            Rank.Six,
            Rank.Seven,
            Rank.Eight,
            Rank.Nine,
            Rank.Ten,
            Rank.Jack,
            Rank.Queen,
            Rank.King
        };

        private static readonly Suit[] suitOrder =
        {
            Suit.Clubs,
            Suit.Spades,
            Suit.Hearts,
            Suit.Diamonds
        };

        public static Int32Rect GetCardRect(Card card)
        {
            var width = 73;
            var height = 97;

            var x = Array.IndexOf(ranksOrder, card.Rank);
            var y = Array.IndexOf(suitOrder, card.Suit);
            return new Int32Rect(x * width, y * height + y, width, height);
        }

        private static readonly Dictionary<int, CroppedBitmap> cardsFrontSources
            = new Dictionary<int, CroppedBitmap>();
    }
}
