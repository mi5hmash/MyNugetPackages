using System.Buffers.Binary;
using System.IO.Compression;

namespace Mi5hmasH.Compressors;

public static class ZLib
{
    /// <param name="bytes">A byte array to process.</param>
    extension(byte[] bytes)
    {
        /// <summary>
        /// Determines whether the underlying byte array represents data in the ZLib compressed format.
        /// </summary>
        /// <returns><see langword="true"/> if the byte array begins with the ZLib magic number; otherwise, <see langword="false"/>.</returns>
        public bool IsZLib() => bytes is [0x78, 0x9C, ..];
        
        /// <summary>
        /// Decompresses a byte array that was compressed using the ZLib compression format.
        /// </summary>
        /// <returns>A byte array containing the decompressed data.</returns>
        public byte[] ZLibDecompress()
        {
            using var msi = new MemoryStream(bytes);
            using var zl = new ZLibStream(msi, CompressionMode.Decompress);
            using var mso = new MemoryStream();
            zl.CopyTo(mso);
            return mso.ToArray();
        }
        
        /// <summary>
        /// Asynchronously decompresses a byte array that was compressed using the ZLib compression format.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is a byte array containing the decompressed data.</returns>
        public async Task<byte[]> ZLibDecompressAsync()
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            await using (var zl = new ZLibStream(msi, CompressionMode.Decompress))
            {
                await zl.CopyToAsync(mso);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Compresses the specified data using the Zlib compression algorithm.
        /// </summary>
        /// <returns>A byte array containing the compressed data.</returns>
        public byte[] ZLibCompress()
        {
            using var mso = new MemoryStream();
            using (var zl = new ZLibStream(mso, CompressionLevel.Optimal))
            {
                zl.Write(bytes);
            }
            return mso.ToArray();
        }
        
        /// <summary>
        /// Asynchronously compresses the specified data using the Zlib compression algorithm.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is a byte array containing the compressed data.</returns>
        public async Task<byte[]> ZLibCompressAsync()
        {
            using var mso = new MemoryStream();
            await using (var zl = new ZLibStream(mso, CompressionLevel.Optimal))
            {
                zl.Write(bytes);
            }
            return mso.ToArray();
        }

        /// <summary>
        /// Compresses the specified data using the Zlib compression algorithm and appends an Adler-32 checksum to the end of the compressed data.
        /// </summary>
        /// <returns>A byte array containing the compressed data with an appended Adler-32 checksum.</returns>
        public byte[] ZLibWithAdler32Compress()
        {
            var adler32Checksum = bytes.ComputeAdler32Checksum();
            var compressedData = bytes.ZLibCompress();
            var compressedDataWithChecksumSpan = GC.AllocateUninitializedArray<byte>(compressedData.Length + 4).AsSpan();
            compressedData.CopyTo(compressedDataWithChecksumSpan);
            BinaryPrimitives.WriteUInt32BigEndian(compressedDataWithChecksumSpan[^4..], adler32Checksum);
            
            return compressedDataWithChecksumSpan.ToArray();
        }
        
        /// <summary>
        /// Asynchronously compresses the specified data using the Zlib compression algorithm and appends an Adler-32 checksum to the end of the compressed data.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is a byte array containing the compressed data with an appended Adler-32 checksum.</returns>
        public async Task<byte[]> ZLibWithAdler32CompressAsync()
        {
            var adler32Checksum = bytes.ComputeAdler32Checksum();
            var compressedData = await bytes.ZLibCompressAsync();
            var compressedDataWithChecksumSpan = GC.AllocateUninitializedArray<byte>(compressedData.Length + 4).AsSpan();
            compressedData.CopyTo(compressedDataWithChecksumSpan);
            BinaryPrimitives.WriteUInt32BigEndian(compressedDataWithChecksumSpan[^4..], adler32Checksum);
            
            return compressedDataWithChecksumSpan.ToArray();
        }

        /// <summary>
        /// Decompresses a byte array that was compressed using the ZLib compression format and verifies the Adler-32 checksum.
        /// </summary>
        /// <returns>A byte array containing the decompressed data.</returns>
        /// <exception cref="InvalidDataException">Thrown when the Adler-32 checksum does not match.</exception>
        public byte[] ZLibWithAdler32Decompress()
        {
            var adler32ChecksumExpected = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan()[^4..]);
            var decompressedDataSpan = bytes.ZLibDecompress();
            var adler32Checksum = decompressedDataSpan.ComputeAdler32Checksum();
            
            return adler32ChecksumExpected != adler32Checksum 
                ? throw new InvalidDataException("Adler-32 checksum mismatch.") 
                : decompressedDataSpan.ToArray();
        }

        /// <summary>
        /// Asynchronously decompresses a byte array that was compressed using the ZLib compression format and verifies the Adler-32 checksum.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is a byte array containing the decompressed data.</returns>
        /// <exception cref="InvalidDataException">Thrown when the Adler-32 checksum does not match.</exception>
        public async Task<byte[]> ZLibWithAdler32DecompressAsync()
        {
            var adler32ChecksumExpected = BinaryPrimitives.ReadUInt32BigEndian(bytes.AsSpan()[^4..]);
            var decompressedData = await bytes.ZLibDecompressAsync();
            var adler32Checksum = decompressedData.ComputeAdler32Checksum();

            return adler32ChecksumExpected != adler32Checksum
                ? throw new InvalidDataException("Adler-32 checksum mismatch.")
                : decompressedData.ToArray();
        }
    }
    
    /// <summary>
    /// Computes the Adler-32 checksum for the specified data.
    /// </summary>
    /// <param name="byteSpan">A read‑only span of bytes whose contents are used to compute the Adler‑32 checksum.</param>
    /// <returns>The computed Adler-32 checksum as an unsigned 32-bit integer.</returns>
    public static uint ComputeAdler32Checksum(this ReadOnlySpan<byte> byteSpan)
    {
        const uint modAdler = 65521;
        uint a = 1, b = 0;
        foreach (var c in byteSpan)
        {
            a = (a + c) % modAdler;
            b = (b + a) % modAdler;
        }
        return (b << 16) | a;
    }
}