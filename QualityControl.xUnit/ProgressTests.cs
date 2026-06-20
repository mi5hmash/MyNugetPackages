using Mi5hmasH.Progress;

namespace QualityControl.xUnit;

public sealed class ProgressTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public ProgressTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }
    
    #region ErrorCounter

    [Fact]
    public void ErrorCounter_AddError_IncrementsErrorCount()
    {
        // Arrange
        var counter = new ErrorCounter();
        // Act
        counter.AddError();
        // Assert
        Assert.Equal(1, counter.Errors);
        Assert.Equal(0, counter.Warnings);
    }

    [Fact]
    public void ErrorCounter_AddErrorWithMessage_DoesNotThrowWhenLoggerIsNull()
    {
        // Arrange
        var counter = new ErrorCounter();
        // Act
        counter.AddError("msg");
        // Assert
        Assert.Equal(1, counter.Errors);
    }

    [Fact]
    public void ErrorCounter_AddWarning_IncrementsWarningCount()
    {
        // Arrange
        var counter = new ErrorCounter();
        // Act
        counter.AddWarning();
        // Assert
        Assert.Equal(1, counter.Warnings);
        Assert.Equal(0, counter.Errors);
    }

    [Fact]
    public void ErrorCounter_AddWarningWithMessage_DoesNotThrowWhenLoggerIsNull()
    {
        // Arrange
        var counter = new ErrorCounter();
        // Act
        counter.AddWarning("msg");
        // Assert
        Assert.Equal(1, counter.Warnings);
    }

    [Fact]
    public void ErrorCounter_ResetErrors_SetsErrorCountToZero()
    {
        // Arrange
        var counter = new ErrorCounter();
        counter.AddWarning();
        counter.AddError();
        // Act
        counter.ResetErrors();
        // Assert
        Assert.Equal(0, counter.Errors);
        Assert.Equal(1, counter.Warnings);
    }

    [Fact]
    public void ErrorCounter_ResetWarnings_SetsWarningCountToZero()
    {
        // Arrange
        var counter = new ErrorCounter();
        counter.AddWarning();
        counter.AddError();
        // Act
        counter.ResetWarnings();
        // Assert
        Assert.Equal(0, counter.Warnings);
        Assert.Equal(1, counter.Errors);
    }

    [Fact]
    public void ErrorCounter_Reset_SetsBothCountsToZero()
    {
        // Arrange
        var counter = new ErrorCounter();
        counter.AddError();
        counter.AddWarning();
        // Act
        counter.Reset();
        // Assert
        Assert.Equal(0, counter.Errors);
        Assert.Equal(0, counter.Warnings);
    }

    [Fact]
    public void ErrorCounter_ToString_ReturnsCorrectFormat()
    {
        // Arrange
        var counter = new ErrorCounter();
        counter.AddError();
        counter.AddWarning();
        counter.AddWarning("msg");
        // Act
        var result = counter.ToString();
        // Assert
        Assert.Equal("[Errors: 1 | Warnings: 2]", result);
    }

    #endregion


    #region ProgressTracker

    [Fact]
    public void ProgressTracker_Constructor_SetsTotalAndStart()
    {
        // Arrange
        var tracker = new ProgressTracker(total: 100, start: 10);
        // Assert
        Assert.Equal(100, tracker.Total);
        Assert.Equal(10, tracker.Current);
    }

    [Fact]
    public void ProgressTracker_Increment_IncreasesCurrentByOne()
    {
        // Arrange
        var tracker = new ProgressTracker(10, 2);
        // Act
        tracker.Increment();
        tracker.Increment();
        // Assert
        Assert.Equal(4, tracker.Current);
    }

    [Fact]
    public void ProgressTracker_Percentage_ComputesCorrectly()
    {
        // Arrange
        var tracker = new ProgressTracker(100, start: 25);
        // Assert
        Assert.Equal(25, tracker.Percentage);
    }

    [Fact]
    public void ProgressTracker_Percentage_RoundsDown()
    {
        // Arrange
        var tracker = new ProgressTracker(3, start: 1);
        // Assert
        Assert.Equal(33, tracker.Percentage); // 1/3 = 33.333 → 33%
    }

    [Fact]
    public void ProgressTracker_PercentageString_ReturnsFormattedValue()
    {
        // Arrange
        var tracker = new ProgressTracker(4, start: 1);
        // Assert
        Assert.Equal("25%", tracker.PercentageString);
    }

    [Fact]
    public void ProgressTracker_ToString_ReturnsExpectedFormat()
    {
        // Arrange
        var tracker = new ProgressTracker(10, start: 3);
        // Assert
        Assert.Equal("[3/10]", tracker.ToString());
    }

    [Fact]
    public async Task ProgressTracker_Increment_IsThreadSafe()
    {
        // Arrange
        var tracker = new ProgressTracker(10_000);
        const int threads = 20;
        const int incrementsPerThread = 5000;
        var tasks = Enumerable.Range(0, threads)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < incrementsPerThread; i++)
                    tracker.Increment();
            }))
            .ToArray();
        // Act
        await Task.WhenAll(tasks);
        const long expected = threads * incrementsPerThread;
        // Assert
        Assert.Equal(expected, tracker.Current);
    }

    [Fact]
    public void ProgressTracker_Percentage_DivisionByZeroTotalDoesNotThrow()
    {
        var tracker = new ProgressTracker(0);
        _ = tracker.Percentage;
    }

    #endregion


    #region ProgressReporter

    private sealed class TestProgress<T> : IProgress<T>
    {
        public List<T> Reports { get; } = [];

        public void Report(T value) => Reports.Add(value);
    }

    [Fact]
    public void ProgressReporter_Report_TextOnlyReportsMessage()
    {
        // Arrange
        const string messageToReport = "hello";
        var text = new TestProgress<string>();
        var reporter = new ProgressReporter(text);
        // Act
        reporter.Report(messageToReport);
        // Assert
        Assert.Single(text.Reports);
        Assert.Equal(messageToReport, text.Reports[0]);
    }

    [Fact]
    public void ProgressReporter_Report_ValueOnlyReportsProgress()
    {
        // Arrange
        const int valueToReport = 42;
        var value = new TestProgress<int>();
        var reporter = new ProgressReporter(value);
        // Act
        reporter.Report(valueToReport);
        // Assert
        Assert.Single(value.Reports);
        Assert.Equal(valueToReport, value.Reports[0]);
    }

    [Fact]
    public void ProgressReporter_Report_BothHandlersReportsMessageAndValue()
    {
        // Arrange
        const int valueToReport = 42;
        const string messageToReport = "hello";
        var text = new TestProgress<string>();
        var value = new TestProgress<int>();
        var reporter = new ProgressReporter(text, value);
        // Act
        reporter.Report(messageToReport, valueToReport);
        // Assert
        Assert.Single(text.Reports);
        Assert.Single(value.Reports);
        Assert.Equal(messageToReport, text.Reports[0]);
        Assert.Equal(valueToReport, value.Reports[0]);
    }

    [Fact]
    public void ProgressReporter_Report_TextOnly_DoesNotThrowWhenValueHandlerIsNull()
    {
        // Arrange
        const string messageToReport = "hello";
        var text = new TestProgress<string>();
        var reporter = new ProgressReporter(text, null);
        // Act
        reporter.Report(messageToReport);
        // Assert
        Assert.Single(text.Reports);
        Assert.Equal(messageToReport, text.Reports[0]);
    }

    [Fact]
    public void ProgressReporter_Report_ValueOnly_DoesNotThrowWhenTextHandlerIsNull()
    {
        // Arrange
        const int valueToReport = 77;
        var value = new TestProgress<int>();
        var reporter = new ProgressReporter(null, value);
        // Act
        reporter.Report(valueToReport);
        // Assert
        Assert.Single(value.Reports);
        Assert.Equal(valueToReport, value.Reports[0]);
    }

    [Fact]
    public void ProgressReporter_Complete_Reports100()
    {
        // Arrange
        var value = new TestProgress<int>();
        var reporter = new ProgressReporter(value);
        // Act
        reporter.Complete();
        // Assert
        Assert.Single(value.Reports);
        Assert.Equal(100, value.Reports[0]);
    }

    [Fact]
    public void ProgressReporter_Complete_WithMessageReportsMessageAnd100()
    {
        // Arrange
        const string messageToReport = "done";
        var text = new TestProgress<string>();
        var value = new TestProgress<int>();
        var reporter = new ProgressReporter(text, value);
        // Act
        reporter.Complete(messageToReport);
        // Assert
        Assert.Single(text.Reports);
        Assert.Single(value.Reports);
        Assert.Equal(messageToReport, text.Reports[0]);
        Assert.Equal(100, value.Reports[0]);
    }

    [Fact]
    public void ProgressReporter_NullHandlers_DoNotThrow()
    {
        // Arrange
        var reporter = new ProgressReporter(null, null);
        // Act
        reporter.Report("ignored");
        reporter.Report(123);
        reporter.Report("msg", 50);
        reporter.Complete();
        reporter.Complete("done");
    }

    #endregion
}