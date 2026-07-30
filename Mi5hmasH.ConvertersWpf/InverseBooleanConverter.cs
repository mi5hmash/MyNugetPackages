using System.Globalization;
using System.Windows.Data;

namespace Mi5hmasH.ConvertersWpf;

/// <summary>
/// A value converter that inverts a boolean value. 
/// If the input is true, it returns false; if the input is false, it returns true. 
/// If the input is not a boolean, it returns the input value unchanged.
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is bool b ? !b : value) ?? throw new InvalidOperationException();

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => (value is bool b ? !b : value) ?? throw new InvalidOperationException();
}