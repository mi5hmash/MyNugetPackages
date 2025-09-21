using System.Globalization;
using System.Reflection;

namespace Mi5hmasH.Logger.Helpers;

/// <summary>
/// Provides functionality for writing objects and headers to a CSV (Comma-Separated Values) output using reflection over public instance properties. Supports configurable culture formatting and resource management.
/// </summary>
/// <param name="writer">The text writer to which CSV data will be written.</param>
/// <param name="culture">The culture to use for formatting values. If null, the invariant culture is used.</param>
/// <param name="leaveOpen">true to leave the underlying writer open after disposing the CsvWriter; otherwise, false.</param>
public class CsvWriter(TextWriter writer, CultureInfo? culture = null, bool leaveOpen = false) : IDisposable, IAsyncDisposable
{
    private readonly TextWriter _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    private readonly CultureInfo _culture = culture ?? CultureInfo.InvariantCulture;
    private bool _disposed;

    /// <summary>
    /// Writes the specified record to the underlying CSV output, using the public instance properties of the record type as columns.
    /// </summary>
    /// <typeparam name="T">The type of the record to write. Must be a reference type with public instance properties to be serialized as CSV columns.</typeparam>
    /// <param name="record">The record instance to write to the CSV output.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="record"/> is null.</exception>
    public void WriteRecord<T>(T record)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var values = new string[properties.Length];

        for (var i = 0; i < properties.Length; i++)
        {
            var value = properties[i].GetValue(record, null);
            values[i] = FormatCsvValue(value);
        }

        _writer.WriteLine(string.Join(",", values));
    }

    /// <summary>
    /// Writes a CSV header line containing the public property names of the specified type.
    /// </summary>
    /// <typeparam name="T">The type whose public instance property names will be written as CSV headers.</typeparam>
    public void WriteHeader<T>()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var headers = new string[properties.Length];

        for (var i = 0; i < properties.Length; i++)
            headers[i] = FormatCsvValue(properties[i].Name);

        _writer.WriteLine(string.Join(",", headers));
    }

    /// <summary>
    /// Formats the specified value for inclusion in a CSV file, applying necessary escaping and quoting according to CSV conventions.
    /// </summary>
    /// <param name="value">The value to format for CSV output. If null, an empty string is returned.</param>
    /// <returns>A string representation of the value formatted for CSV. If the value contains special characters such as quotes,
    /// commas, or line breaks, it is enclosed in double quotes and internal quotes are escaped. Returns an empty string if the value is null.</returns>
    private string FormatCsvValue(object? value)
    {
        if (value == null) return string.Empty;
        var str = Convert.ToString(value, _culture);
        if (str == null) return string.Empty;
        if (str.Contains('"') || str.Contains(',') || str.Contains('\n') || str.Contains('\r'))
            str = '"' + str.Replace("\"", "\"\"") + '"';
        return str;
    }

    /// <summary>
    /// Releases all resources used by the current instance of the class.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        if (!leaveOpen) _writer.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Asynchronously releases the unmanaged resources used by the current instance and, optionally, disposes the underlying writer.
    /// </summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        if (!leaveOpen)
        {
            if (_writer is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            else if (_writer is IDisposable disposable)
                disposable.Dispose();
        }
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}