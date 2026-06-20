using System.Diagnostics.CodeAnalysis;
using Mi5hmasH.Logger.Enums;

namespace Mi5hmasH.Logger.Models;

public interface ILogEntry
{
    DateTime Timestamp { get; set; }
    LogSeverityEnum LogLevel { get; set; }
    string? Group { get; set; }
    string Message { get; set; }

    int GetSize();
    void Set(ILogEntry other, bool copyTimestamp = true);
    bool Equals(ILogEntry? other);
    bool Equals([NotNullWhen(true)] object? obj);
    int GetHashCodeStable();
    int GetHashCode();
}