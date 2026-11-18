using System.Globalization;

namespace WarehouseNFC.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b && b)
            {
                return parameter?.ToString() switch
                {
                    "highlight" => Color.FromArgb("#1976D2"),
                    _ => Color.FromArgb("#C62828")
                };
            }
            return parameter?.ToString() switch
            {
                "highlight" => Colors.Transparent,
                _ => Color.FromArgb("#43A047")
            };
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => false;
    }
}