using Mi5hmasH.ConvertersWpf;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace QualityControl.xUnit;

public sealed class ConvertersWpfTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public ConvertersWpfTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }
    
    #region FileNameWithoutExtensionConverter

    [Theory]
    [InlineData(@"C:\temp\file.txt", "file")]
    [InlineData("/usr/local/bin/script.sh", "script")]
    [InlineData("justName.ext", "justName")]
    [InlineData("noExtension", "noExtension")]
    public void FileNameWithoutExtensionConverter_Convert_ValidStringReturnsFileNameWithoutExtension(string input, string expected)
    {
        // Arrange
        FileNameWithoutExtensionConverter converter = new();
        // Act
        var result = converter.Convert(input, typeof(string), null, CultureInfo.InvariantCulture);
        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FileNameWithoutExtensionConverter_Convert_NullReturnsEmptyString()
    {
        // Arrange
        FileNameWithoutExtensionConverter converter = new();
        // Act
        var result = converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FileNameWithoutExtensionConverter_Convert_EmptyOrWhitespaceReturnsEmptyString(string input)
    {
        // Arrange
        FileNameWithoutExtensionConverter converter = new();
        // Act
        var result = converter.Convert(input, typeof(string), null, CultureInfo.InvariantCulture);
        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FileNameWithoutExtensionConverter_Convert_NonStringReturnsEmptyString()
    {
        // Arrange
        FileNameWithoutExtensionConverter converter = new();
        // Act
        var result = converter.Convert(123, typeof(string), null, CultureInfo.InvariantCulture);
        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void FileNameWithoutExtensionConverter_ConvertBack_AlwaysReturnsBindingDoNothing()
    {
        // Arrange
        FileNameWithoutExtensionConverter converter = new();
        // Act
        var result = converter.ConvertBack("anything", typeof(string), null, CultureInfo.InvariantCulture);
        // Assert
        Assert.Equal(Binding.DoNothing, result);
    }

    #endregion


    #region InverseBooleanConverter

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void InverseBooleanConverter_Convert_InvertsBoolean(bool input, bool expected)
    {
        // Arrange
        InverseBooleanConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        // Act
        var result = converter.Convert(input, typeof(bool), null, culture);
        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void InverseBooleanConverter_Convert_NonBoolReturnsValueUnchanged()
    {
        // Arrange
        InverseBooleanConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        const string input = "hello";
        // Act
        var result = converter.Convert(input, typeof(string), null, culture);
        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void InverseBooleanConverter_Convert_NullThrowsInvalidOperationException()
    {
        // Arrange
        InverseBooleanConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            converter.Convert(null, typeof(bool), null, culture));
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void InverseBooleanConverter_ConvertBack_InvertsBoolean(bool input, bool expected)
    {
        // Arrange
        InverseBooleanConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        // Act
        var result = converter.ConvertBack(input, typeof(bool), null, culture);
        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void InverseBooleanConverter_ConvertBack_NonBoolReturnsValueUnchanged()
    {
        // Arrange
        InverseBooleanConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        const int input = 123;
        // Act
        var result = converter.ConvertBack(input, typeof(int), null, culture);
        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void InverseBooleanConverter_ConvertBack_NullThrowsInvalidOperationException()
    {
        // Arrange
        InverseBooleanConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            converter.ConvertBack(null, typeof(bool), null, culture));
    }

    #endregion


    #region Base64ToImageConverter

    private static string CreateTestPngBase64()
    {
        // Create a 1x1 pixel PNG in memory
        var bitmap = new WriteableBitmap(1, 1, 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
        bitmap.Lock();
        bitmap.Unlock();

        using var ms = new MemoryStream();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        encoder.Save(ms);

        return Convert.ToBase64String(ms.ToArray());
    }

    [Fact]
    public void Base64ToImageConverter_Convert_ValidBase64ReturnsBitmapImage()
    {
        // Arrange
        Base64ToImageConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        var base64 = CreateTestPngBase64();
        // Act
        var result = converter.Convert(base64, typeof(BitmapImage), null, culture);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<BitmapImage>(result);
    }

    [Fact]
    public void Base64ToImageConverter_Convert_InvalidBase64ReturnsNull()
    {
        // Arrange
        Base64ToImageConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        // Act
        var result = converter.Convert("NOT_BASE64", typeof(BitmapImage), null, culture);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Base64ToImageConverter_Convert_NonStringReturnsNull()
    {
        // Arrange
        Base64ToImageConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        const int input = 123;
        // Act
        var result = converter.Convert(input, typeof(BitmapImage), null, culture);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Base64ToImageConverter_ConvertBack_ValidBitmapImageReturnsBase64String()
    {
        // Arrange
        Base64ToImageConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        var base64 = CreateTestPngBase64();
        var bitmap = (BitmapImage)converter.Convert(base64, typeof(BitmapImage), null, culture)!;
        // Act
        var result = converter.ConvertBack(bitmap, typeof(string), null, culture);
        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
        var roundTrip = (BitmapImage)converter.Convert(result, typeof(BitmapImage), null, culture)!;
        Assert.NotNull(roundTrip);
    }

    [Fact]
    public void Base64ToImageConverter_ConvertBack_InvalidTypeThrowsNotSupportedException()
    {
        // Arrange
        Base64ToImageConverter converter = new();
        var culture = CultureInfo.InvariantCulture;
        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            converter.ConvertBack("not a bitmap", typeof(string), null, culture));
    }

    #endregion
}