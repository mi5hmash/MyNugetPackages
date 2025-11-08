using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Mi5hmasH.Utilities.Converters;

namespace Mi5hmasH.Logger.Models;

/// <summary>
/// Represents a log entry containing information about an event, including its severity, timestamp, message, and optional grouping.
/// </summary>
/// <remarks>This class is used to encapsulate details about a single log event, such as its severity
/// level, associated message, and optional grouping for categorization. It provides constructors for creating log
/// entries with varying levels of detail  and supports equality comparison and stable hash code generation.</remarks>
public class LogEntry : IEquatable<LogEntry>
{
    /// <summary>
    /// The timestamp of the log entry.
    /// </summary>
    [TypeConverter(typeof(DateTimeFormatConverter))]
    [DateTimeFormat("yyyy-MM-dd HH:mm:ss.fff")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The severity level for logging messages.
    /// </summary>
    public SimpleLogger.LogSeverity LogLevel { get; set; }

    /// <summary>
    /// The name of the group associated with the entity.
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// The message associated with the current operation.
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with the specified log severity and message.
    /// </summary>
    /// <param name="logLevel">The severity level of the log entry.</param>
    /// <param name="message">The message associated with the log entry. Cannot be null.</param>
    public LogEntry(SimpleLogger.LogSeverity logLevel, string message)
    {
        LogLevel = logLevel;
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> class with the specified group, log level, and message.
    /// </summary>
    /// <remarks>Use this constructor to create a log entry with specific details, such as grouping
    /// logs by category or specifying the severity level for filtering purposes.</remarks>
    /// <param name="group">The group or category associated with the log entry. This value cannot be null or empty.</param>
    /// <param name="logLevel">The severity level of the log entry, indicating its importance or urgency.</param>
    /// <param name="message">The message content of the log entry. This value cannot be null or empty.</param>
    public LogEntry(string group, SimpleLogger.LogSeverity logLevel, string message)
    {
        Group = group;
        LogLevel = logLevel;
        Message = message;
    }

    /// <summary>
    /// Default constructor for CsvHelper
    /// </summary>
    public LogEntry() { }

    /// <summary>
    /// Calculates the total size of the log entry in bytes.
    /// </summary>
    /// <returns>The total size of the log entry in bytes.</returns>
    public int GetSize()
        => 8 + sizeof(SimpleLogger.LogSeverity) + (Group?.Length ?? 0) + Message.Length;

    /// <summary>
    /// Copies the log level, group, and message from the specified <see cref="LogEntry"/> instance, optionally copying the timestamp.
    /// </summary>
    /// <param name="other">The <see cref="LogEntry"/> instance whose values are to be copied.</param>
    /// <param name="copyTimestamp">Indicates whether to copy the timestamp from <paramref name="other"/>.</param>
    public void Set(LogEntry other, bool copyTimestamp = true)
    {
        LogLevel = other.LogLevel;
        Group = other.Group;
        Message = other.Message;
        if (copyTimestamp)
            Timestamp = other.Timestamp;
    }

    public bool Equals(LogEntry? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;

        var sc = StringComparer.Ordinal;
        return Timestamp == other.Timestamp &&
               LogLevel == other.LogLevel &&
               sc.Equals(Group, other.Group) &&
               sc.Equals(Message, other.Message);
    }

    public int GetHashCodeStable()
    {
        var hc = new HashCode();
        var sc = StringComparer.Ordinal;
        // Add fields to the hash code computation
        hc.Add(Timestamp);
        hc.Add(LogLevel);
        hc.Add(Group, sc);
        hc.Add(Message, sc);
        return hc.ToHashCode();
    }

    // This is a workaround to avoid the default GetHashCode() implementation in objects where all fields are mutable.
    private readonly Guid _uniqueId = Guid.NewGuid();
    public override int GetHashCode()
        => _uniqueId.GetHashCode();
    
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is LogEntry castedObj && Equals(castedObj);

    public static bool operator ==(LogEntry? left, LogEntry? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(LogEntry? left, LogEntry? right)
        => !(left == right);
}