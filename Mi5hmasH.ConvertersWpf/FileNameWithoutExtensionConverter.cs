using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Mi5hmasH.ConvertersWpf;

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