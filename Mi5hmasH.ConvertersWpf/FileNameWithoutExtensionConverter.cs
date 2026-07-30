using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Mi5hmasH.ConvertersWpf;

/// <summary>
/// A value converter that extracts the file name without its extension from a given file path string.
/// </summary>
public class FileNameWithoutExtensionConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrWhiteSpace(s))
            return Path.GetFileNameWithoutExtension(s);
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Binding.DoNothing;
}