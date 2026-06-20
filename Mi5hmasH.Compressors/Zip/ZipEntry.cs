namespace Mi5hmasH.Compressors.Zip;

/// <summary>
/// Represents a single entry in a ZIP archive.
/// </summary>
public class ZipEntry(byte[] entryData, string entryPath)
{
    /// <summary>
    /// Path of the entry within the ZIP archive, including optional folder structure.
    /// </summary>
    public string EntryPath { get; set; } = entryPath;

    /// <summary>
    /// The data associated with the entry.
    /// </summary>
    public byte[] EntryData { get; set; } = entryData;
}