using System.Globalization;
using System.Windows.Data;

namespace Mi5hmasH.ConvertersWpf;

public class UppercaseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => value?.ToString()?.ToUpperInvariant();

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => null;
}
