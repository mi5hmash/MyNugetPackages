using System.IO.Compression;

namespace Mi5hmasH.Compressors;

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

/// <summary>
/// Provides static methods for compressing and decompressing data using the ZIP archive format.
/// </summary>
public static class Zip
{
    /// <summary>
    /// Compresses <paramref name="bytes"/> into a ZIP archive with a specified <paramref name="entryName"/>.
    /// </summary>
    /// <param name="bytes">The byte array to compress.</param>
    /// <param name="entryName">The name of the entry within the ZIP archive.</param>
    /// <param name="compressionLevel">Specifies the level of compression applied to an entry. Options include Optimal, Fastest, and NoCompression.</param>
    /// <returns>The compressed ZIP archive as a byte array.</returns>
    public static byte[] ZipCompress(this byte[] bytes, string entryName = "data", CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        using var mso = new MemoryStream();
        using (var zipArchive = new ZipArchive(mso, ZipArchiveMode.Create, true))
        {
            var entry = zipArchive.CreateEntry(entryName, compressionLevel);
            using var entryStream = entry.Open();
            entryStream.Write(bytes, 0, bytes.Length);
        }
        return mso.ToArray();
    }

    /// <summary>
    /// Compresses multiple <see cref="ZipEntry"/> objects into a single ZIP archive.
    /// </summary>
    /// <param name="entries">A list of <see cref="ZipEntry"/> objects, each representing a file to include in the archive.</param>
    /// <param name="compressionLevel">Specifies the level of compression applied to each entry in the archive. Options include Optimal, Fastest, and NoCompression.</param>
    /// <returns>A byte array representing the compressed ZIP archive.</returns>
    public static byte[] ZipCompress(List<ZipEntry> entries, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        using var mso = new MemoryStream();
        using (var zipArchive = new ZipArchive(mso, ZipArchiveMode.Create, true))
        {
            foreach (var entry in entries)
            {
                var zipEntry = zipArchive.CreateEntry(entry.EntryPath, compressionLevel);
                using var entryStream = zipEntry.Open();
                entryStream.Write(entry.EntryData, 0, entry.EntryData.Length);
            }
        }
        return mso.ToArray();
    }

    /// <summary>
    /// Asynchronously compresses <paramref name="bytes"/> into a ZIP archive with a specified <paramref name="entryName"/>.
    /// </summary>
    /// <param name="bytes">The byte array to compress.</param>
    /// <param name="entryName">The name of the entry within the ZIP archive.</param>
    /// <param name="compressionLevel">Specifies the level of compression applied to an entry. Options include Optimal, Fastest, and NoCompression.</param>
    /// <returns>The compressed ZIP archive as a byte array.</returns>
    public static async Task<byte[]> ZipCompressAsync(this byte[] bytes, string entryName = "data", CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        using var mso = new MemoryStream();
        await using (var zipArchive = new ZipArchive(mso, ZipArchiveMode.Create, true))
        {
            var entry = zipArchive.CreateEntry(entryName, compressionLevel);
            await using var entryStream = await entry.OpenAsync();
            await entryStream.WriteAsync(bytes);
        }
        return mso.ToArray();
    }

    /// <summary>
    /// Asynchronously compresses multiple <see cref="ZipEntry"/> objects into a single ZIP archive.
    /// </summary>
    /// <param name="entries">A list of <see cref="ZipEntry"/> objects, each representing a file to include in the archive.</param>
    /// <param name="compressionLevel">Specifies the level of compression applied to each entry in the archive. Options include Optimal, Fastest, and NoCompression.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a byte array representing the compressed ZIP archive.</returns>
    public static async Task<byte[]> ZipCompressAsync(List<ZipEntry> entries, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        using var mso = new MemoryStream();
        await using (var zipArchive = new ZipArchive(mso, ZipArchiveMode.Create, true))
        {
            foreach (var entry in entries)
            {
                var zipEntry = zipArchive.CreateEntry(entry.EntryPath, compressionLevel);
                await using var entryStream = await zipEntry.OpenAsync();
                await entryStream.WriteAsync(entry.EntryData);
            }
        }
        return mso.ToArray();
    }

    /// <summary>
    /// Decompresses a ZIP archive <paramref name="zipBytes"/> and extracts the contents of the specified <paramref name="entryName"/>.
    /// </summary>
    /// <param name="zipBytes">The ZIP archive as a byte array.</param>
    /// <param name="entryName">The name of the entry to extract.</param>
    /// <returns>The decompressed entry as a byte array.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static byte[] ZipDecompress(this byte[] zipBytes, string entryName = "data")
    {
        using var msi = new MemoryStream(zipBytes);
        using var zipArchive = new ZipArchive(msi, ZipArchiveMode.Read);
        var entry = zipArchive.GetEntry(entryName) 
                    ?? throw new InvalidOperationException($"Entry '{entryName}' not found in the ZIP archive.");

        using var entryStream = entry.Open();
        using var mso = new MemoryStream();
        entryStream.CopyTo(mso);
        return mso.ToArray();
    }

    /// <summary>
    /// Asynchronously decompresses a ZIP archive <paramref name="zipBytes"/> and extracts the contents of the specified <paramref name="entryName"/>.
    /// </summary>
    /// <param name="zipBytes">The ZIP archive as a byte array.</param>
    /// <param name="entryName">The name of the entry to extract.</param>
    /// <returns>The decompressed entry as a byte array.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task<byte[]> ZipDecompressAsync(this byte[] zipBytes, string entryName = "data")
    {
        using var msi = new MemoryStream(zipBytes);
        await using var zipArchive = new ZipArchive(msi, ZipArchiveMode.Read);
        var entry = zipArchive.GetEntry(entryName) 
                    ?? throw new InvalidOperationException($"Entry '{entryName}' not found in the ZIP archive.");

        await using var entryStream = await entry.OpenAsync();
        using var mso = new MemoryStream();
        await entryStream.CopyToAsync(mso);
        return mso.ToArray();
    }
}