using System.IO.Compression;

namespace Mi5hmasH.Compressors.Zip;

/// <summary>
/// Provides static methods for compressing and decompressing data using the ZIP archive format.
/// </summary>
public static class Zip
{
    /// <param name="entries">A list of <see cref="ZipEntry"/> objects, each representing a file process.</param>
    extension(List<ZipEntry> entries)
    {
        /// <summary>
        /// Compresses multiple <see cref="ZipEntry"/> objects into a single ZIP archive.
        /// </summary>
        /// <param name="compressionLevel">Specifies the level of compression applied to each entry in the archive. Options include Optimal, Fastest, and NoCompression.</param>
        /// <returns>A byte array representing the compressed ZIP archive.</returns>
        public byte[] ZipCompress(CompressionLevel compressionLevel = CompressionLevel.Optimal)
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
        /// Asynchronously compresses multiple <see cref="ZipEntry"/> objects into a single ZIP archive.
        /// </summary>
        /// <param name="compressionLevel">Specifies the level of compression applied to each entry in the archive. Options include Optimal, Fastest, and NoCompression.</param>
        /// <returns>A task representing the asynchronous operation. The task result is a byte array representing the compressed ZIP archive.</returns>
        public async Task<byte[]> ZipCompressAsync(CompressionLevel compressionLevel = CompressionLevel.Optimal)
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
    }

    /// <param name="bytes">The bytes to process.</param>
    extension(byte[] bytes)
    {
        /// <summary>
        /// Decompresses a ZIP archive <paramref name="bytes"/> and extracts the contents of the specified <paramref name="entryName"/>.
        /// </summary>
        /// <param name="entryName">The name of the entry to extract.</param>
        /// <returns>The decompressed entry as a byte array.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] ZipDecompress(string entryName = "data")
        {
            using var msi = new MemoryStream(bytes);
            using var zipArchive = new ZipArchive(msi, ZipArchiveMode.Read);
            var entry = zipArchive.GetEntry(entryName) 
                        ?? throw new InvalidOperationException($"Entry '{entryName}' not found in the ZIP archive.");

            using var entryStream = entry.Open();
            using var mso = new MemoryStream();
            entryStream.CopyTo(mso);
            return mso.ToArray();
        }

        /// <summary>
        /// Asynchronously decompresses a ZIP archive <paramref name="bytes"/> and extracts the contents of the specified <paramref name="entryName"/>.
        /// </summary>
        /// <param name="entryName">The name of the entry to extract.</param>
        /// <returns>The decompressed entry as a byte array.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<byte[]> ZipDecompressAsync(string entryName = "data")
        {
            using var msi = new MemoryStream(bytes);
            await using var zipArchive = new ZipArchive(msi, ZipArchiveMode.Read);
            var entry = zipArchive.GetEntry(entryName) 
                        ?? throw new InvalidOperationException($"Entry '{entryName}' not found in the ZIP archive.");

            await using var entryStream = await entry.OpenAsync();
            using var mso = new MemoryStream();
            await entryStream.CopyToAsync(mso);
            return mso.ToArray();
        }

        /// <summary>
        /// Asynchronously compresses <paramref name="bytes"/> into a ZIP archive with a specified <paramref name="entryName"/>.
        /// </summary>
        /// <param name="entryName">The name of the entry within the ZIP archive.</param>
        /// <param name="compressionLevel">Specifies the level of compression applied to an entry. Options include Optimal, Fastest, and NoCompression.</param>
        /// <returns>The compressed ZIP archive as a byte array.</returns>
        public async Task<byte[]> ZipCompressAsync(string entryName = "data", CompressionLevel compressionLevel = CompressionLevel.Optimal)
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
        /// Compresses <paramref name="bytes"/> into a ZIP archive with a specified <paramref name="entryName"/>.
        /// </summary>
        /// <param name="entryName">The name of the entry within the ZIP archive.</param>
        /// <param name="compressionLevel">Specifies the level of compression applied to an entry. Options include Optimal, Fastest, and NoCompression.</param>
        /// <returns>The compressed ZIP archive as a byte array.</returns>
        public byte[] ZipCompress(string entryName = "data", CompressionLevel compressionLevel = CompressionLevel.Optimal)
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
    }
}