namespace Mi5hmasH.Progress;

/// <summary>
/// Tracks progress of a long‑running operation using a thread‑safe counter.
/// </summary>
/// <param name="total">Total number of steps required to complete the operation.</param>
/// <param name="start">Initial progress value (defaults to 0).</param>
public sealed class ProgressTracker(long total, long start = 0)
{
    private long _current = start;

    /// <summary>
    /// Gets the total number of steps required to complete the operation.
    /// </summary>
    public long Total { get; } = total;

    /// <summary>
    /// Atomically increments the current progress value by 1.
    /// </summary>
    /// <returns>The incremented value.</returns>
    public void Increment() => Interlocked.Increment(ref _current);

    /// <summary>
    /// Gets the current progress value in a thread‑safe manner.
    /// </summary>
    public long Current => Volatile.Read(ref _current);

    /// <summary>
    /// Gets the current progress percentage (0–100).
    /// </summary>
    public int Percentage
        => (int)((double)Current / Total * 100);

    /// <summary>
    /// Gets the progress percentage formatted as a string with a percent sign.
    /// </summary>
    public string PercentageString => $"{Percentage}%";

    /// <summary>
    /// Returns a string representation of the progress in the form "[current/total]".
    /// </summary>
    public override string ToString()
        => $"[{Current}/{Total}]";
}