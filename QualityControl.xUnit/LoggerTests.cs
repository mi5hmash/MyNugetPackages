using Mi5hmasH.Logger;
using Mi5hmasH.Logger.Providers;

namespace QualityControl.xUnit;

public sealed class LoggerTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public LoggerTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
        // Setup
        RecreateLogsPath();
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }

    #region LOGS_PATH

    private readonly string _logsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_logsTests");
    
    private void RecreateLogsPath()
    {
        try { Directory.Delete(_logsPath, true); }
        catch { /* ignored */ }
        Directory.CreateDirectory(_logsPath);
    }

    #endregion
    
    [Theory]
    [InlineData(3, 1, 1, "_1")]
    [InlineData(3, 0, 3, "_2")]
    public void SimpleLogger_FileLimit_ShouldNotExceedTheLimit(int logFilesToCreate, int fileNumberLimit, int expectedFilesNumber, string suffix)
    {
        // ARRANGE
        // Create a SimpleLogger instance
        var logger = new SimpleLogger
        {
            LoggedAppName = "SimpleLoggerApp",
            LoggedAppVersion = new Version(1, 2, 3, 4)
        };
        
        // ACT
        // Configure FileLogProvider
        var fileLogProvider = new FileLogProvider(_logsPath, fileNumberLimit)
        {
            LogFileNamePrefix = $"SimpleLoggerFileLimit{suffix}"
        };

        // Create log files up to the specified limit
        for (var i = 0; i < logFilesToCreate; i++) 
            fileLogProvider.CreateLogFile();
        
        logger.AddProvider(fileLogProvider);
        logger.Flush();

        // Get the count of log files created
        var logFilesCount = fileLogProvider.GetLogFileList().Count;

        // ASSERT
        Assert.Equal(expectedFilesNumber, logFilesCount);
    }
    
    [Fact]
    public async Task SimpleLogger_AsyncLogging_ShouldLogEveryEntry()
    {
        // ARRANGE
        // Create a SimpleLogger instance
        var logger = new SimpleLogger
        {
            LoggedAppName = "SimpleLoggerApp",
            LoggedAppVersion = new Version(1, 2, 3, 4)
        };
        // Configure FileLogProvider
        var fileLogProvider = new FileLogProvider(_logsPath, 1)
        {
            LogFileNamePrefix = "SimpleLoggerAsync",
            MaxBufferSize = 100
        };
        fileLogProvider.CreateLogFile();
        logger.AddProvider(fileLogProvider);

        // ACT
        await Task.Run(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                logger.LogInfo($"Info message {i}", "Group 1");
            }
        }, TestContext.Current.CancellationToken);

        await Task.Run(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                logger.LogTrace($"Trace message {i}", "Group 2");
            }
        }, TestContext.Current.CancellationToken);

        var logDataBeforeFlush = (await File.ReadAllLinesAsync(fileLogProvider.GetCurrentLogFilePath(), TestContext.Current.CancellationToken)).Length;
        await logger.FlushAsync();
        var logDataAfterFlush = (await File.ReadAllLinesAsync(fileLogProvider.GetCurrentLogFilePath(), TestContext.Current.CancellationToken)).Length;

        // ASSERT
        Assert.True(logDataBeforeFlush > 10);
        const int expectedLogEntries = 2003;
        Assert.Equal(expectedLogEntries, logDataAfterFlush);
    }

    [Fact]
    public void SimpleLogger_MinimumSeverity_ShouldNotLogLowerSeverityLogs()
    {
        // ARRANGE
        // Create a SimpleLogger instance
        var logger = new SimpleLogger
        {
            LoggedAppName = "SimpleLoggerApp",
            LoggedAppVersion = new Version(1, 2, 3, 4),
            MinSeverityLevel = SimpleLogger.LogSeverity.Info
        };
        // Configure FileLogProvider
        var fileLogProvider = new FileLogProvider(_logsPath, 1)
        {
            LogFileNamePrefix = "SimpleLoggerSeverity"
        };
        fileLogProvider.CreateLogFile();
        logger.AddProvider(fileLogProvider);

        // ACT
        logger.LogTrace("Trace message", "Group 1");
        logger.LogDebug("Debug message", "Group 1");
        logger.LogInfo("Info message", "Group 2");
        logger.LogWarning("Warning message", "Group 2");
        logger.LogError("Error message", "Group 1");
        logger.LogCritical("Critical message");
        logger.Flush();

        var logData = File.ReadAllText(fileLogProvider.GetCurrentLogFilePath());

        // ASSERT
        Assert.DoesNotContain("Trace", logData);
        Assert.DoesNotContain("Debug", logData);
        Assert.Contains("Info", logData);
    }
}