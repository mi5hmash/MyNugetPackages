using System.IO.Compression;

namespace Mi5hmasH.Compressors;

public static class Brotli
{
    /// <param name="bytes">The byte array to be processed.</param>
    extension(byte[] bytes)
    {
        /// <summary>
        /// Compresses <paramref name="bytes"/> with Brotli.
        /// </summary>
        /// <param name="compressionLevel">The level of compression to apply (default: Optimal).</param>
        /// <returns>The compressed byte array.</returns>
        public byte[] BrotliCompress(CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            using var mso = new MemoryStream();
            using (var bs = new BrotliStream(mso, compressionLevel))
            {
                bs.Write(bytes, 0, bytes.Length);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Asynchronously compress <paramref name="bytes"/> with Brotli.
        /// </summary>
        /// <param name="compressionLevel">The level of compression to apply (default: Optimal).</param>
        /// <returns>The compressed byte array.</returns>
        public async Task<byte[]> BrotliCompressAsync(CompressionLevel compressionLevel = CompressionLevel.Optimal)
        {
            using var mso = new MemoryStream();
            await using (var bs = new BrotliStream(mso, compressionLevel))
            {
                bs.Write(bytes, 0, bytes.Length);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Decompresses <paramref name="bytes"/> with Brotli.
        /// </summary>
        /// <returns>The decompressed byte array.</returns>
        public byte[] BrotliDecompress()
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var bs = new BrotliStream(msi, CompressionMode.Decompress))
            {
                bs.CopyTo(mso);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Asynchronously decompress <paramref name="bytes"/> compressed with Brotli.
        /// </summary>
        /// <returns>The decompressed byte array.</returns>
        public async Task<byte[]> BrotliDecompressAsync()
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            await using (var bs = new BrotliStream(msi, CompressionMode.Decompress))
            {
                await bs.CopyToAsync(mso);
            }
            return mso.ToArray();
        }
    }
}