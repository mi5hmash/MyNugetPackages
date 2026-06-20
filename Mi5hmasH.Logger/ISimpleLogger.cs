using Mi5hmasH.Logger.Enums;
using Mi5hmasH.Logger.LogProvidersFactory;

namespace Mi5hmasH.Logger;

public interface ISimpleLogger
{
    string LoggedAppName { get; set; }
    Version LoggedAppVersion { get; set; }
    LogSeverityEnum MinSeverityLevel { get; set; }

    void AddProvider(ILogProvider provider);
    void ClearProviders();
    void LogTrace(string message, string? group = null);
    void LogDebug(string message, string? group = null);
    void LogInfo(string message, string? group = null);
    void LogWarning(string message, string? group = null);
    void LogError(string message, string? group = null);
    void LogCritical(string message, string? group = null);
    void Flush();
    Task FlushAsync();
}