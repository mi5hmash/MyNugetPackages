using Mi5hmasH.WpfHelper;

namespace QualityControl.xUnit;

public sealed class WpfHelperTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public WpfHelperTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }

#if DEBUG
    [Fact]
    public void Debug_GetAccentColorsCode_ResultShouldNotBeNull()
    {
        // Act
        var result = WpfThemeAccent.GetAccentColorsCode();
        _output.WriteLine(result);
        // Assert
        Assert.NotNull(result);
    }
#endif
}