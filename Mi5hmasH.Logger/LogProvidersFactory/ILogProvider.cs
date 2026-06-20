using Mi5hmasH.Logger.Models;

namespace Mi5hmasH.Logger.LogProvidersFactory;

/// <summary>
/// Defines a provider for logging messages and events.
/// </summary>
public interface ILogProvider
{
    void Log(ILogEntry entry);
    Task LogAsync(ILogEntry entry);
    void Flush();
    Task FlushAsync();
}