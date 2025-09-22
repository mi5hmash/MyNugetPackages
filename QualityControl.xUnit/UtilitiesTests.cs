using Mi5hmasH.Utilities;

namespace QualityControl.xUnit;

public sealed class UtilitiesTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public UtilitiesTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }

    #region StringHelpers

    [Theory]
    [InlineData("My Monday 1910", "My Monday ")]
    [InlineData("My M0nday 1910a", "My M0nday 1910a")]
    [InlineData("M1", "M")]
    [InlineData("1910", "")]
    public void RemoveSuffixNumbers_ShouldReturnExpectedResult(string inputString, string expected)
    {
        // Act
        var result = inputString.RemoveSuffixNumbers();

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("My Monday 1910", 4, "My M")]
    [InlineData("My Monday 1910", 99, "My Monday 1910")]
    [InlineData("My Monday 1910", 0, "")]
    [InlineData("My Monday 1910", -2, "")]
    public void Left_ShouldReturnExpectedResult(string inputString, int numberOfCharacters, string expected)
    {
        // Act
        var result = inputString.Left(numberOfCharacters);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("My Monday 1910", 4, "1910")]
    [InlineData("My Monday 1910", 99, "My Monday 1910")]
    [InlineData("My Monday 1910", 0, "")]
    [InlineData("My Monday 1910", -2, "")]
    public void Right_ShouldReturnExpectedResult(string inputString, int numberOfCharacters, string expected)
    {
        // Act
        var result = inputString.Right(numberOfCharacters);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion


    #region RegexValidation

    [Theory]
    [InlineData("myEmail@gmail.com", "gmail.com")]
    [InlineData("myEmail@external.gmail.com", "gmail.com")]
    public void RegexValidation_CustomEmail_ShouldReturnTrue(string inputEmailAddress, string domain)
    {
        // Act
        var result = inputEmailAddress.IsEmail(RegexValidation.EmailCustomPattern(domain));

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("myEmail@gmail.com", "onet.pl")]
    [InlineData("myEmail@external.gmail.com", "onet.pl")]
    public void RegexValidation_CustomEmail_ShouldReturnFalse(string inputEmailAddress, string domain)
    {
        // Act
        var result = inputEmailAddress.IsEmail(RegexValidation.EmailCustomPattern(domain));

        // Assert
        Assert.False(result);
    }

    #endregion
}