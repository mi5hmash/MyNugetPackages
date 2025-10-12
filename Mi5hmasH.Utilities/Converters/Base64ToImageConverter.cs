using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Mi5hmasH.Utilities.Converters;

public class Base64ToImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        try
        {
            if (value is not string s)
                return null;

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(System.Convert.FromBase64String(s));
            bi.CacheOption = BitmapCacheOption.OnLoad; // Optional for better performance
            bi.EndInit();

            return bi;
        }
        catch
        {
            // Handle invalid Base64 string or image loading errors
            return null;
        }
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is not BitmapImage bitmapImage)
            throw new NotSupportedException("Only BitmapImage is supported for ConvertBack.");

        // Convert the BitmapImage to a Base64 string
        byte[] imageBytes;
        using (var ms = new MemoryStream())
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(ms);
            imageBytes = ms.ToArray();
        }

        return System.Convert.ToBase64String(imageBytes);
    }
}