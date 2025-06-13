using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RystBrewery.Software.Converters
{
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as string ?? "";

            return status switch
            {
                "Idle" => Brushes.Gray,
                "Running" => Brushes.Yellow,
                "Completed" => Brushes.Green,
                "Paused" => Brushes.Black,
                "Stopped" => Brushes.Blue,
                "Error" => Brushes.Red,
                _ => Brushes.Gray
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException("One-way binding only.");
    }
}
