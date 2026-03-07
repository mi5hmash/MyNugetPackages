using System.IO.Compression;

namespace Mi5hmasH.Compressors;

/// <summary>
/// Provides static methods for compressing and decompressing byte arrays using the GZip data format.
/// </summary>
public static class GZip
{
    /// <param name="bytes">The data to compress as a byte array.</param>
    extension(byte[] bytes)
    {
        /// <summary>
        /// Determines whether the underlying byte array represents data in the Gzip compressed format.
        /// </summary>
        /// <returns><see langword="true"/> if the byte array begins with the Gzip magic number; otherwise, <see langword="false"/>.</returns>
        public bool IsGzip() => bytes is [0x1F, 0x8B, ..];

        /// <summary>
        /// Compresses a byte array using GZip compression.
        /// </summary>
        /// <param name="compressionLevel">The level of compression to apply, ranging from NoCompression to Optimal.</param>
        /// <returns>A byte array containing the compressed data.</returns>
        public byte[] GzipCompress(CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            using var mso = new MemoryStream();
            using (var gz = new GZipStream(mso, compressionLevel))
            {
                gz.Write(bytes, 0, bytes.Length);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Asynchronously compresses a byte array using GZip compression.
        /// </summary>
        /// <param name="compressionLevel">The level of compression to apply, ranging from NoCompression to Optimal.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is a byte array containing the compressed data.</returns>
        public async Task<byte[]> GZipCompressAsync(CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            using var mso = new MemoryStream();
            await using (GZipStream gz = new(mso, compressionLevel))
            {
                gz.Write(bytes, 0, bytes.Length);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Decompresses a byte array that was compressed using GZip compression.
        /// </summary>
        /// <returns>A byte array containing the decompressed data.</returns>
        public byte[] GzipDecompress()
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gz = new GZipStream(msi, CompressionMode.Decompress))
            {
                gz.CopyTo(mso);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Asynchronously decompresses a byte array that was compressed using GZip compression.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is a byte array containing the decompressed data.</returns>
        public async Task<byte[]> GZipDecompressAsync()
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            await using (var gz = new GZipStream(msi, CompressionMode.Decompress))
            {
                await gz.CopyToAsync(mso);
            }
            return mso.ToArray();
        }
    }
}