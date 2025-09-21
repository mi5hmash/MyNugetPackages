using Mi5hmasH.Logger.Models;

namespace Mi5hmasH.Logger.Providers;

/// <summary>
/// Provides a logging implementation that updates a status bar with log messages.
/// </summary>
/// <param name="updateStatusBar"></param>
public class StatusBarLogProvider(Action<string> updateStatusBar) : ILogProvider
{
    public void Log(LogEntry entry)
    {
        updateStatusBar.Invoke(string.IsNullOrEmpty(entry.Group)
            ? $"{entry.Message}"
            : $"[{entry.Group}] {entry.Message}");
    }

    public async Task LogAsync(LogEntry entry)
    {
        Log(entry);
        await Task.CompletedTask;
    }

    public void Flush()
    {
        // No operation needed for status bar updates, as they are immediate.
    }

    public Task FlushAsync()
    {
        // No operation needed for status bar updates, as they are immediate.
        return Task.CompletedTask;
    }
}