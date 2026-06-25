# Disclaimer
This NuGet package was created exclusively for use in my own open‑source projects published on my GitHub profile.
While it is publicly available, it is not intended to serve as a general‑purpose library, nor is it designed or documented for external production use.
Feel free to explore the code, but please keep in mind that its primary purpose is to support my personal project ecosystem.

# Usage
## Progress Tracker with 10 tasks
```csharp
// Initialize progress tracker with 10 tasks
var progressTracker = new ProgressTracker(10);
```

## Progress Tracker with 10 tasks which starts from the 2nd task
```csharp
// Initialize progress tracker with 10 tasks which starts from the 2nd task
var progressTracker = new ProgressTracker(10, 2);
```

## Increment progress
```csharp
// Increment progress
progressTracker.Increment();
```

## Error Counter
```csharp
// Initialize error counter
var errorCounter = new ErrorCounter();
```

## Error Counter with a logger
```csharp
// Initialize error counter with a logger
var logger = new SimpleLogger
{
    LoggedAppName = "MyApp",
    LoggedAppVersion = new Version("1.0.0.0")
};
var errorCounter = new ErrorCounter(logger);
```

## Error Counter - add an error and a warning with a message
```csharp
// Error
errorCounter.AddError();

// Warning with a message
errorCounter.AddWarning($"{progressTracker} This is a warning.");

// Warning and a message tied to a specific TaskId
errorCounter.AddWarning($"{progressTracker} This is a warning for a specific TaskId.", "TaskId");
```

## Reset Error Counter
```csharp
// Reset warnings only
errorCounter.ResetWarnings();

// Reset errors only
errorCounter.ResetErrors();

// Reset Error Counter
errorCounter.Reset();
```

## Progress Reporter for WPF
```csharp
#region PROGRESS_REPORTER
[ObservableProperty] private int _progressValue;
[ObservableProperty] private string _progressText = "Loading...";
private readonly ProgressReporter _progressReporter;
#endregion

public MainWindowViewModel()
{
    // Initialize ProgressReporter
    _progressReporter = new ProgressReporter(
        new Progress<string>(s => ProgressText = s),
        new Progress<int>(i => ProgressValue = i)
    );
}
```

## Progress Reporter for ConsoleApplication
```csharp
// Initialize ProgressReporter
var progressReporter = new ProgressReporter(null, null);
```

## Progress Reporter - Report progress with a message
```csharp
// Finalize setup
progressReporter.Report("Ready", 100);
```