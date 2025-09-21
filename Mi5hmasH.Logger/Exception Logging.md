# Exception logging
### Console or Windows application
```csharp
static void Main(string[] args)
{
    // Initialize APP_INFO
    var appInfo = new MyAppInfo("ExceptionTest");
    // Initialize LOGGER
    var logger = new SimpleLogger
    {
        LoggedAppName = appInfo.Name,
        LoggedAppVersion = new Version(MyAppInfo.Version)
    };
    // Configure ConsoleLogProvider
    var consoleLogProvider = new ConsoleLogProvider();
    logger.AddProvider(consoleLogProvider);
    // Configure FileLogProvider
    var fileLogProvider = new FileLogProvider(MyAppInfo.RootPath, 2);
    fileLogProvider.CreateLogFile();
    logger.AddProvider(fileLogProvider);
    // Add event handler for unhandled exceptions
    AppDomain.CurrentDomain.UnhandledException += (_, e) =>
    {
        if (e.ExceptionObject is not Exception exception) return;
        var logEntry = new LogEntry(SimpleLogger.LogSeverity.Critical, $"Unhandled Exception: {exception}");
        fileLogProvider.Log(logEntry);
        fileLogProvider.Flush();
    };
    // Flush log providers on process exit
    AppDomain.CurrentDomain.ProcessExit += (_, _)
        => logger.Flush();

    // Continue app execution...
}
```

### ASP.NET Core app
```csharp
public class ExceptionLoggingService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception exception)
            {
                var logEntry = new LogEntry(SimpleLogger.LogSeverity.Critical, $"Unhandled Exception: {exception}");
                FileLogProviderInstance.Log(logEntry);
            }
        };
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
```