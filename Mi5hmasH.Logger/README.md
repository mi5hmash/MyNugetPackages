# Disclaimer
This NuGet package was created exclusively for use in my own open‑source projects published on my GitHub profile.  
While it is publicly available, it is not intended to serve as a general‑purpose library, nor is it designed or documented for external production use.  
Feel free to explore the code, but please keep in mind that its primary purpose is to support my personal project ecosystem.  
  
# Usage
## Console or Windows application
```csharp
static void Main(string[] args)
{
    // Initialize LOGGER
    var logger = new SimpleLogger
    {
        LoggedAppName = "MyConsoleApp",
        LoggedAppVersion = new Version("1.0.0.0")
    };
    
    // CONFIGURE LOG PROVIDERS
    // Configure ConsoleLogProvider
    var consoleLogProvider = new ConsoleLogProvider();
    logger.AddProvider(consoleLogProvider);
    // Configure FileLogProvider
    var fileLogProvider = new FileLogProvider(AppDomain.CurrentDomain.BaseDirectory, 2);
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

## ASP.NET Core app
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

## Logging a message
```csharp
logger.LogInfo("Info message");
// For immediate output, you can force the logger to flush any buffered messages
logger.Flush();
```