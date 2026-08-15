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
    
    public static TheoryData<string, string> Base64EncodingTheories =>
        new()
        {
            { "Hello World!", "SGVsbG8gV29ybGQh" },
            { "Zażółć gęślą jaźń!", "WmHFvMOzxYLEhyBnxJnFm2zEhSBqYcW6xYQh" }
        };

    [Fact]
    public void Base64_EncodingAscii_ShouldReturnExpectedResult()
    {
        // Arrange
        var first = Base64EncodingTheories.First();
        var inputString = first.Data.Item1;
        var expected = first.Data.Item2;
        
        // Act
        var result = inputString.B64Encode(Encoding.ASCII);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Base64_DecodingAscii_ShouldReturnExpectedResult()
    {
        // Arrange
        var first = Base64EncodingTheories.First();
        var inputString = first.Data.Item1;
        var expected = first.Data.Item2;

        // Act
        var result = inputString.B64Decode(Encoding.ASCII);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(Base64EncodingTheories))]
    public void Base64_EncodingUtf8_ShouldReturnExpectedResult(string inputString, string expected)
    {
        // Act
        var result = inputString.B64Encode(Encoding.UTF8);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(Base64EncodingTheories))]
    public void Base64_DecodingUtf8_ShouldReturnExpectedResult(string expected, string inputString)
    {
        // Act
        var result = inputString.B64Decode(Encoding.UTF8);

        // Assert
        Assert.Equal(expected, result);
    }
}