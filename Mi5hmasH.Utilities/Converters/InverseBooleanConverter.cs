using System.Globalization;
using System.Windows.Data;

namespace Mi5hmasH.Utilities.Converters;

public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is bool b ? !b : value) ?? throw new InvalidOperationException();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is bool b ? !b : value) ?? throw new InvalidOperationException();
}