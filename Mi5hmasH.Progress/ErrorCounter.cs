using Mi5hmasH.Logger;

namespace Mi5hmasH.Progress;

/// <summary>
/// An error and warning tracker.
/// </summary>
public sealed class ErrorCounter(ISimpleLogger? logger = null)
{
    private readonly Lock _lock = new();

    /// <summary>
    /// Gets the number of errors.
    /// </summary>
    public int Errors { get; private set; }

    /// <summary>
    /// Gets the number of warnings.
    /// </summary>
    public int Warnings { get; private set; }

    /// <summary>
    /// Increments the error counter by one.
    /// </summary>
    public void AddError()
    {
        lock (_lock)
        {
            Errors++;
        }
    }

    /// <summary>
    /// Increments the error counter by one and logs the error message if a logger is provided.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="group">The group associated with the error.</param>
    public void AddError(string message, string? group = null)
    {
        lock (_lock)
        {
            AddError();
            logger?.LogError(message, group);
        }
    }

    /// <summary>
    /// Increments the warning counter by one.
    /// </summary>
    public void AddWarning()
    {
        lock (_lock)
        {
            Warnings++;
        }
    }

    /// <summary>
    /// Increments the warning counter by one and logs the warning message if a logger is provided.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    /// <param name="group">The group associated with the warning.</param>
    public void AddWarning(string message, string? group = null)
    {
        lock (_lock)
        {
            AddWarning();
            logger?.LogWarning(message, group);
        }
    }

    /// <summary>
    /// Resets the error counter to zero.
    /// </summary>
    public void ResetErrors()
    {
        lock (_lock)
        {
            Errors = 0;
        }
    }

    /// <summary>
    /// Resets the warning counter to zero.
    /// </summary>
    public void ResetWarnings()
    {
        lock (_lock)
        {
            Warnings = 0;
        }
    }

    /// <summary>
    /// Resets both errors and warnings to zero.
    /// </summary>
    public void Reset()
    {
        ResetErrors();
        ResetWarnings();
    }

    /// <summary>
    /// Returns a string representation of the error and warning counts.
    /// </summary>
    /// <returns>A string in the format "[Errors: X | Warnings: Y]".</returns>
    public override string ToString()
    {
        lock (_lock)
        {
            return $"[Errors: {Errors} | Warnings: {Warnings}]";
        }
    }
}