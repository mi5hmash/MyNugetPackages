extern alias UtilitiesAlias;
using UtilitiesAlias::Mi5hmasH.Utilities.Helpers;

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

    private const string TestString = "My Monday 1910";

    [Theory]
    [InlineData(TestString, "My Monday ")]
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
    [InlineData(TestString, 4, "My M")]
    [InlineData(TestString, 99, "My Monday 1910")]
    [InlineData(TestString, 0, "")]
    [InlineData(TestString, -2, "")]
    public void Left_ShouldReturnExpectedResult(string inputString, int numberOfCharacters, string expected)
    {
        // Act
        var result = inputString.Left(numberOfCharacters);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(TestString, 4, "1910")]
    [InlineData(TestString, 99, "My Monday 1910")]
    [InlineData(TestString, 0, "")]
    [InlineData(TestString, -2, "")]
    public void Right_ShouldReturnExpectedResult(string inputString, int numberOfCharacters, string expected)
    {
        // Act
        var result = inputString.Right(numberOfCharacters);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion
    
    #region RegexValidation

    private const string TestEmail = "myEmail@gmail.com";
    private const string TestEmailExternal = "myEmail@external.gmail.com";
    private const string GmailDomain = "gmail.com";
    private const string OnetDomain = "onet.pl";

    [Theory]
    [InlineData(TestEmail, GmailDomain)]
    [InlineData(TestEmailExternal, GmailDomain)]
    public void RegexValidation_CustomEmail_ShouldReturnTrue(string inputEmailAddress, string domain)
    {
        // Act
        var result = inputEmailAddress.IsEmail(RegexValidation.EmailCustomPattern(domain));

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData(TestEmail, OnetDomain)]
    [InlineData(TestEmailExternal, OnetDomain)]
    public void RegexValidation_CustomEmail_ShouldReturnFalse(string inputEmailAddress, string domain)
    {
        // Act
        var result = inputEmailAddress.IsEmail(RegexValidation.EmailCustomPattern(domain));

        // Assert
        Assert.False(result);
    }

    #endregion

    #region VersionExtensions
    
    [Theory]
    [InlineData("1.2.3", "1.2.3", true)]
    [InlineData("1.2.4", "1.2.3", true)]
    [InlineData("1.2.2", "1.2.3", false)]
    public void IsAtLeast_WorksCorrectly(string version, string min, bool expected)
        => Assert.Equal(expected, version.IsAtLeast(min));

    [Theory]
    [InlineData("1.2.4", "1.2.3", true)]
    [InlineData("1.2.3", "1.2.3", false)]
    [InlineData("1.2.2", "1.2.3", false)]
    public void IsNewerThan_WorksCorrectly(string version, string other, bool expected)
        => Assert.Equal(expected, version.IsNewerThan(other));

    [Theory]
    [InlineData("1.2.2", "1.2.3", true)]
    [InlineData("1.2.3", "1.2.3", false)]
    [InlineData("1.2.4", "1.2.3", false)]
    public void IsOlderThan_WorksCorrectly(string version, string other, bool expected)
        => Assert.Equal(expected, version.IsOlderThan(other));

    [Theory]
    [InlineData("1.2.3", "1.2.0", "1.3.0", true)]
    [InlineData("1.2.0", "1.2.0", "1.3.0", true)]
    [InlineData("1.3.0", "1.2.0", "1.3.0", true)]
    [InlineData("1.2.3", "1.2.0", "1.3.0", true, false)]
    [InlineData("1.2.0", "1.2.0", "1.3.0", false, false)]
    [InlineData("1.3.0", "1.2.0", "1.3.0", false, false)]
    public void IsBetween_WorksCorrectly(string version, string min, string max, bool expected, bool inclusive = true) 
        => Assert.Equal(expected, version.IsBetween(min, max, inclusive));

    [Theory]
    [InlineData("1.2.3", "1.2.3", VersionComparisonResult.Equal)]
    [InlineData("1.2.4", "1.2.3", VersionComparisonResult.Newer)]
    [InlineData("1.2.2", "1.2.3", VersionComparisonResult.Older)]
    public void CompareToVersion_WorksCorrectly(string version, string other, VersionComparisonResult expected) 
        => Assert.Equal(expected, version.CompareToVersion(other));

    #endregion
}