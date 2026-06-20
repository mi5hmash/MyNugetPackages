using Mi5hmasH.Logger;

namespace Mi5hmasH.Progress;

/// <summary>
/// An error and warning tracker.
/// </summary>
public sealed class ErrorCounter(ISimpleLogger? logger = null)
{
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
    public void AddError() => Errors++;

    /// <summary>
    /// Increments the error counter by one and logs the error message if a logger is provided.
    /// </summary>
    /// <param name="message">The error message to log.</param>
    /// <param name="group">The group associated with the error.</param>
    public void AddError(string message, string? group = null)
    {
        AddError();
        logger?.LogError(message, group);
    }

    /// <summary>
    /// Increments the warning counter by one.
    /// </summary>
    public void AddWarning() => Warnings++;

    /// <summary>
    /// Increments the warning counter by one and logs the warning message if a logger is provided.
    /// </summary>
    /// <param name="message">The warning message to log.</param>
    /// <param name="group">The group associated with the warning.</param>
    public void AddWarning(string message, string? group = null)
    {
        AddWarning();
        logger?.LogWarning(message, group);
    }

    /// <summary>
    /// Resets the error counter to zero.
    /// </summary>
    public void ResetErrors() => Errors = 0;

    /// <summary>
    /// Resets the warning counter to zero.
    /// </summary>
    public void ResetWarnings() => Warnings = 0;

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
        => $"[Errors: {Errors} | Warnings: {Warnings}]";
}