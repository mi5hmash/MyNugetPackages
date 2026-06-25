namespace Mi5hmasH.Compressors.Zip;

/// <summary>
/// Represents a single entry in a ZIP archive.
/// </summary>
public sealed record ZipEntry(
    byte[] EntryData,
    string EntryPath
);