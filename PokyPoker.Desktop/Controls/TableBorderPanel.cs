using System;
using System.Windows;
using System.Windows.Controls;

namespace PokyPoker.Desktop.Controls
{
    public class TableBorderPanel : Panel
    {
        public static readonly DependencyProperty FlatteningProperty = DependencyProperty.Register(
            "Flattening", typeof(double), typeof(TableBorderPanel), new PropertyMetadata(default(double)));

        public double Flattening
        {
            get => (double) GetValue(FlatteningProperty);
            set => SetValue(FlatteningProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement elem in Children)
                elem.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            return base.MeasureOverride(availableSize);
        }

        private static double Clamp(double value, double min, double max)
        {
#if DEBUG
            if (max < min)
                throw new ArgumentException("Max < Min");
#endif

            return value < min ? min : value > max ? max : value;
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
                return finalSize;

            var angle = 0.0;
            var incrementalAngularSpace = 360.0 / Children.Count * (Math.PI / 180);

            var radiusX = finalSize.Width / 2.4;
            var radiusY = finalSize.Height / 2.4;
            var flat = Math.Abs(Flattening);
            var maxY = flat > 1 ? radiusY : radiusY * flat;
            
            foreach (UIElement elem in Children)
            {
                var x = Math.Cos(angle) * radiusX;
                var y = Clamp(-Math.Sin(angle) * radiusY, -maxY, maxY);

                //Calculate the point on the circle for the element
                var childPoint = new Point(x, y);

                //Offsetting the point to the available rectangular area which is FinalSize.
                var actualChildPoint = new Point(finalSize.Width / 2 + childPoint.X - elem.DesiredSize.Width / 2,
                    finalSize.Height / 2 + childPoint.Y - elem.DesiredSize.Height / 2);

                //Call Arrange method on the child element by giving the calculated point as the placementPoint.
                elem.Arrange(new Rect(actualChildPoint.X, actualChildPoint.Y, elem.DesiredSize.Width,
                    elem.DesiredSize.Height));

                //Calculate the new _angle for the next element
                angle += incrementalAngularSpace;
            }

            return finalSize;
        }
    }
}
