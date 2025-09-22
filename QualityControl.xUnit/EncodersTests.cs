using Mi5hmasH.Encoders;
using System.Text;

namespace QualityControl.xUnit;

public sealed class EncodersTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public EncodersTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }
    
    [Fact]
    public void Base64_EncodingAscii_ShouldReturnExpectedResult()
    {
        // Arrange
        const string inputString = "Hello World!";
        const string expected = "SGVsbG8gV29ybGQh";
        
        // Act
        var result = inputString.B64Encode(Encoding.ASCII);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Base64_DecodingAscii_ShouldReturnExpectedResult()
    {
        // Arrange
        const string inputString = "SGVsbG8gV29ybGQh";
        const string expected = "Hello World!";

        // Act
        var result = inputString.B64Decode(Encoding.ASCII);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Hello World!", "SGVsbG8gV29ybGQh")]
    [InlineData("Zażółć gęślą jaźń!", "WmHFvMOzxYLEhyBnxJnFm2zEhSBqYcW6xYQh")]
    public void Base64_EncodingUtf8_ShouldReturnExpectedResult(string inputString, string expected)
    {
        // Act
        var result = inputString.B64Encode(Encoding.UTF8);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("SGVsbG8gV29ybGQh", "Hello World!")]
    [InlineData("WmHFvMOzxYLEhyBnxJnFm2zEhSBqYcW6xYQh", "Zażółć gęślą jaźń!")]
    public void Base64_DecodingUtf8_ShouldReturnExpectedResult(string inputString, string expected)
    {
        // Act
        var result = inputString.B64Decode(Encoding.UTF8);

        // Assert
        Assert.Equal(expected, result);
    }
}