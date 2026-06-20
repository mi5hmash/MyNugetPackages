using System.ComponentModel;
using Mi5hmasH.Converters;
using System.Globalization;

namespace QualityControl.xUnit;

public sealed class ConvertersTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public ConvertersTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }

    #region DateTimeFormatConverter

    private class TestContext : ITypeDescriptorContext
    {
        public PropertyDescriptor? PropertyDescriptor { get; init; }

        public IContainer? Container => null;
        public object? Instance => null;
        public void OnComponentChanged() { }
        public bool OnComponentChanging() => true;
        public object? GetService(Type serviceType) => null;
    }

    [DateTimeFormat("dd-MM-yyyy HH:mm")]
    public DateTime CustomFormattedDate { get; set; }

    public DateTime DefaultFormattedDate { get; set; }

    private PropertyDescriptor? GetProperty(string name)
        => TypeDescriptor.GetProperties(typeof(ConvertersTests))[name];

    [Fact]
    public void DateTimeFormatConverter_CanConvertFrom_StringReturnsTrue()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        // Assert
        Assert.True(converter.CanConvertFrom(typeof(string)));
    }

    [Fact]
    public void DateTimeFormatConverter_CanConvertTo_StringReturnsTrue()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        // Assert
        Assert.True(converter.CanConvertTo(typeof(string)));
    }

    [Fact]
    public void DateTimeFormatConverter_ConvertFrom_ValidStringWithCustomFormatReturnsDateTime()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        var context = new TestContext
        {
            PropertyDescriptor = GetProperty(nameof(CustomFormattedDate))
        };
        const string input = "25-12-2024 14:30";

        // Act
        var result = converter.ConvertFrom(context, CultureInfo.InvariantCulture, input);

        // Assert
        Assert.IsType<DateTime>(result);
        Assert.Equal(new DateTime(2024, 12, 25, 14, 30, 0), result);
    }

    [Fact]
    public void DateTimeFormatConverter_ConvertFrom_ValidStringWithDefaultFormatReturnsDateTime()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        var context = new TestContext
        {
            PropertyDescriptor = GetProperty(nameof(DefaultFormattedDate))
        };
        var dt = new DateTime(2024, 1, 2, 03, 04, 05, 123);
        var input = dt.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
        // Act
        var result = converter.ConvertFrom(context, CultureInfo.InvariantCulture, input);
        // Assert
        Assert.Equal(dt, result);
    }

    [Fact]
    public void DateTimeFormatConverter_ConvertFrom_InvalidStringThrowsException()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        var context = new TestContext
        {
            PropertyDescriptor = GetProperty(nameof(CustomFormattedDate))
        };
        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            converter.ConvertFrom(context, CultureInfo.InvariantCulture, "invalid-date"));
    }

    [Fact]
    public void DateTimeFormatConverter_ConvertTo_StringWithCustomFormatReturnsFormattedString()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        var context = new TestContext
        {
            PropertyDescriptor = GetProperty(nameof(CustomFormattedDate))
        };
        var dt = new DateTime(2024, 12, 25, 14, 30, 0);
        // Act
        var result = converter.ConvertTo(context, CultureInfo.InvariantCulture, dt, typeof(string));
        // Assert
        Assert.Equal("25-12-2024 14:30", result);
    }

    [Fact]
    public void DateTimeFormatConverter_ConvertTo_StringWithDefaultFormatReturnsFormattedString()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        var context = new TestContext
        {
            PropertyDescriptor = GetProperty(nameof(DefaultFormattedDate))
        };
        var dt = new DateTime(2024, 1, 2, 03, 04, 05, 123);
        // Act
        var result = converter.ConvertTo(context, CultureInfo.InvariantCulture, dt, typeof(string));
        // Assert
        Assert.Equal("2024-01-02 03:04:05.123", result);
    }

    [Fact]
    public void DateTimeFormatConverter_ConvertTo_NonStringTypeDelegatesToBase()
    {
        // Arrange
        var converter = new DateTimeFormatConverter();
        var context = new TestContext
        {
            PropertyDescriptor = GetProperty(nameof(DefaultFormattedDate))
        };
        var dt = DateTime.Now;
        // Act & Assert
        Assert.Throws<NotSupportedException>(() =>
            converter.ConvertTo(context, CultureInfo.InvariantCulture, dt, typeof(int)));
    }

    #endregion
}