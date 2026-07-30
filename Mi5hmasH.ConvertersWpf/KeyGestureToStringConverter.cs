using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace Mi5hmasH.ConvertersWpf;

/// <summary>
/// A value converter that converts between a KeyGesture and its string representation.
/// </summary>
public class KeyGestureToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) 
        => value is KeyGesture gesture 
            ? gesture.GetDisplayStringForCulture(culture) 
            : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string s || string.IsNullOrWhiteSpace(s))
            return null;

        var converter = new KeyGestureConverter();
        try { return converter.ConvertFromString(null, culture, s); }
        catch { return null; }
    }
}
