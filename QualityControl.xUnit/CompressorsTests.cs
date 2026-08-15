using System.Buffers.Binary;
using Mi5hmasH.Compressors;
using Mi5hmasH.Compressors.Zip;
using System.Text;

namespace QualityControl.xUnit;

public sealed class CompressorsTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public CompressorsTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }

    public static TheoryData<string> CompressTheories =>
    [
        "Hello World!",
        "Zażółć gęślą jaźń!"
    ];

    #region Brotli

    public static TheoryData<string, byte[]> BrotliCompressTheories =>
        new()
        {
            { "Hello World!", [0x8B, 0x05, 0x80, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x20, 0x57, 0x6F, 0x72, 0x6C, 0x64, 0x21, 0x03] },
            { "Zażółć gęślą jaźń!", [0x0B, 0x0D, 0x80, 0x5A, 0x61, 0xC5, 0xBC, 0xC3, 0xB3, 0xC5, 0x82, 0xC4, 0x87, 0x20, 0x67, 0xC4, 0x99, 0xC5, 0x9B, 0x6C, 0xC4, 0x85, 0x20, 0x6A, 0x61, 0xC5, 0xBA, 0xC5, 0x84, 0x21, 0x03] }
        };

    [Theory]
    [MemberData(nameof(BrotliCompressTheories))]
    public void Brotli_BrotliCompress_ShouldCompressCorrectly(string inputString, byte[] expectedData)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = originalData.BrotliCompress();

        // Assert
        Assert.Equal(expectedData, compressedData);
    }

    [Theory]
    [MemberData(nameof(BrotliCompressTheories))]
    public async Task Brotli_BrotliCompressAsync_ShouldCompressCorrectly(string inputString, byte[] expectedData)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = await originalData.BrotliCompressAsync();

        // Assert
        Assert.Equal(expectedData, compressedData);
    }
    
    [Theory]
    [MemberData(nameof(CompressTheories))]
    public void Brotli_BrotliCompress_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = originalData.BrotliCompress();
        var decompressedData = compressedData.BrotliDecompress();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }

    [Theory]
    [MemberData(nameof(CompressTheories))]
    public async Task Brotli_BrotliCompressAsync_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = await originalData.BrotliCompressAsync();
        var decompressedData = await compressedData.BrotliDecompressAsync();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }

    [Fact]
    public void Brotli_BrotliCompress_EmptyInput_ShouldReturnEmptyOutput()
    {
        // Arrange
        byte[] emptyData = [];

        // Act
        var compressedData = emptyData.BrotliCompress();
        var decompressedData = compressedData.BrotliDecompress();

        // Assert
        Assert.Empty(decompressedData);
    }

    [Fact]
    public async Task Brotli_BrotliCompressAsync_EmptyInput_ShouldReturnEmptyOutput()
    {
        // Arrange
        byte[] emptyData = [];

        // Act
        var compressedData = await emptyData.BrotliCompressAsync();
        var decompressedData = await compressedData.BrotliDecompressAsync();

        // Assert
        Assert.Empty(decompressedData);
    }

    #endregion
    
    #region GZip

    public static TheoryData<string, byte[]> GZipCompressTheories =>
        new()
        {
            { "Hello World!", [0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0xF3, 0x48, 0xCD, 0xC9, 0xC9, 0x57, 0x08, 0xCF, 0x2F, 0xCA, 0x49, 0x51, 0x04, 0x00, 0xA3, 0x1C, 0x29, 0x1C, 0x0C, 0x00, 0x00, 0x00] },
            { "Zażółć gęślą jaźń!", [0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x8B, 0x4A, 0x3C, 0xBA, 0xE7, 0xF0, 0xE6, 0xA3, 0x4D, 0x47, 0xDA, 0x15, 0xD2, 0x8F, 0xCC, 0x3C, 0x3A, 0x3B, 0xE7, 0x48, 0xAB, 0x42, 0x56, 0xE2, 0xD1, 0x5D, 0x47, 0x5B, 0x14, 0x01, 0x52, 0xC1, 0xB6, 0xA9, 0x1B, 0x00, 0x00, 0x00] }
        };
    
    [Theory]
    [MemberData(nameof(GZipCompressTheories))]
    public void Gzip_GzipCompress_ShouldCompressCorrectly(string inputString, byte[] expectedData)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = originalData.GzipCompress();

        // Assert
        Assert.Equal(expectedData, compressedData);
    }

    [Theory]
    [MemberData(nameof(GZipCompressTheories))]
    public async Task Gzip_GZipCompressAsync_ShouldCompressCorrectly(string inputString, byte[] expectedData)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = await originalData.GZipCompressAsync();

        // Assert
        Assert.Equal(compressedData, expectedData);
    }

    [Theory]
    [MemberData(nameof(CompressTheories))]
    public void Gzip_GzipCompress_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = originalData.GzipCompress();
        var decompressedData = compressedData.GzipDecompress();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }

    [Theory]
    [MemberData(nameof(CompressTheories))]
    public async Task Gzip_GZipCompressAsync_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = await originalData.GZipCompressAsync();
        var decompressedData = await compressedData.GZipDecompressAsync();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }

    [Fact]
    public void Gzip_GzipCompress_EmptyInput_ShouldReturnEmptyOutput()
    {
        // Arrange
        byte[] emptyData = [];

        // Act
        var compressedData = emptyData.GzipCompress();
        var decompressedData = compressedData.GzipDecompress();

        // Assert
        Assert.Empty(decompressedData);
    }

    [Fact]
    public async Task Gzip_GZipCompressAsync_EmptyInput_ShouldReturnEmptyOutput()
    {
        // Arrange
        byte[] emptyData = [];

        // Act
        var compressedData = await emptyData.GZipCompressAsync();
        var decompressedData = await compressedData.GZipDecompressAsync();

        // Assert
        Assert.Empty(decompressedData);
    }

    #endregion
    
    #region ZLib
    
    public static TheoryData<string, byte[]> ZLibCompressTheories =>
        new()
        {
            { "Hello World!", [0x78, 0x9C, 0xF3, 0x48, 0xCD, 0xC9, 0xC9, 0x57, 0x08, 0xCF, 0x2F, 0xCA, 0x49, 0x51, 0x04, 0x00, 0x1C, 0x49, 0x04, 0x3E] },
            { "Zażółć gęślą jaźń!", [0x78, 0x9C, 0x8B, 0x4A, 0x3C, 0xBA, 0xE7, 0xF0, 0xE6, 0xA3, 0x4D, 0x47, 0xDA, 0x15, 0xD2, 0x8F, 0xCC, 0x3C, 0x3A, 0x3B, 0xE7, 0x48, 0xAB, 0x42, 0x56, 0xE2, 0xD1, 0x5D, 0x47, 0x5B, 0x14, 0x01, 0xDA, 0x9F, 0x0F, 0x12] }
        };
    
    [Theory]
    [MemberData(nameof(ZLibCompressTheories))]
    public void ZLib_ZLibCompress_ShouldCompressCorrectly(string inputString, byte[] expectedData)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = originalData.ZLibCompress();

        // Assert
        Assert.Equal(expectedData, compressedData);
    }

    [Theory]
    [MemberData(nameof(ZLibCompressTheories))]
    public async Task ZLib_ZLibCompressAsync_ShouldCompressCorrectly(string inputString, byte[] expectedData)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = await originalData.ZLibCompressAsync();

        // Assert
        Assert.Equal(compressedData, expectedData);
    }

    [Theory]
    [MemberData(nameof(CompressTheories))]
    public void ZLib_ZLibCompress_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = originalData.ZLibCompress();
        var decompressedData = compressedData.ZLibDecompress();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }

    [Theory]
    [MemberData(nameof(CompressTheories))]
    public async Task ZLib_ZLibCompressAsync_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = await originalData.ZLibCompressAsync();
        var decompressedData = await compressedData.ZLibDecompressAsync();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }

    [Fact]
    public void ZLib_ZLibCompress_EmptyInput_ShouldReturnEmptyOutput()
    {
        // Arrange
        byte[] emptyData = [];

        // Act
        var compressedData = emptyData.ZLibCompress();
        var decompressedData = compressedData.ZLibDecompress();

        // Assert
        Assert.Empty(decompressedData);
    }

    [Fact]
    public async Task ZLib_ZLibCompressAsync_EmptyInput_ShouldReturnEmptyOutput()
    {
        // Arrange
        byte[] emptyData = [];

        // Act
        var compressedData = await emptyData.ZLibCompressAsync();
        var decompressedData = await compressedData.ZLibDecompressAsync();

        // Assert
        Assert.Empty(decompressedData);
    }
    
    [Theory]
    [MemberData(nameof(ZLibCompressTheories))]
    public void ZLib_ZLibWithAdler32Compress_ShouldCompressCorrectly(string inputString, byte[] expectedCompressed)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedWithAdler = originalData.ZLibWithAdler32Compress();

        // Assert
        Assert.NotNull(compressedWithAdler);
        Assert.True(compressedWithAdler.Length >= expectedCompressed.Length + 4);

        // Pierwsze N bajtów muszą być identyczne z ZLibCompress()
        Assert.Equal(expectedCompressed, compressedWithAdler[..expectedCompressed.Length]);

        // Ostatnie 4 bajty to Adler32 skompresowanych danych
        var expectedAdler = originalData.ComputeAdler32Checksum();
        var actualAdler = BinaryPrimitives.ReadUInt32BigEndian(compressedWithAdler.AsSpan()[^4..]);

        Assert.Equal(expectedAdler, actualAdler);
    }

    [Theory]
    [MemberData(nameof(ZLibCompressTheories))]
    public async Task ZLib_ZLibWithAdler32CompressAsync_ShouldCompressCorrectly(string inputString, byte[] expectedCompressed)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedWithAdler = await originalData.ZLibWithAdler32CompressAsync();

        // Assert
        Assert.NotNull(compressedWithAdler);
        Assert.True(compressedWithAdler.Length >= expectedCompressed.Length + 4);

        Assert.Equal(expectedCompressed, compressedWithAdler[..expectedCompressed.Length]);

        var expectedAdler = originalData.ComputeAdler32Checksum();
        var actualAdler = BinaryPrimitives.ReadUInt32BigEndian(compressedWithAdler.AsSpan()[^4..]);

        Assert.Equal(expectedAdler, actualAdler);
    }
    
    [Theory]
    [MemberData(nameof(CompressTheories))]
    public void ZLib_ZLibWithAdler32Compress_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedWithAdler = originalData.ZLibWithAdler32Compress();
        var decompressed = compressedWithAdler.ZLibWithAdler32Decompress();

        // Assert
        Assert.NotNull(compressedWithAdler);
        Assert.NotEmpty(compressedWithAdler);
        Assert.Equal(originalData, decompressed);
    }

    [Theory]
    [MemberData(nameof(CompressTheories))]
    public async Task ZLib_ZLibWithAdler32CompressAsync_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedWithAdler = await originalData.ZLibWithAdler32CompressAsync();
        var decompressed = await compressedWithAdler.ZLibWithAdler32DecompressAsync();

        // Assert
        Assert.NotNull(compressedWithAdler);
        Assert.NotEmpty(compressedWithAdler);
        Assert.Equal(originalData, decompressed);
    }
    
    [Fact]
    public void ZLib_ZLibWithAdler32Decompress_InvalidChecksum_ShouldThrow()
    {
        // Arrange
        var originalData = "test"u8.ToArray();
        var compressedWithAdler = originalData.ZLibWithAdler32Compress();
        
        // Act & Assert
        compressedWithAdler[^1] ^= 0xFF;
        Assert.Throws<InvalidDataException>(compressedWithAdler.ZLibWithAdler32Decompress);
    }

    [Fact]
    public async Task ZLib_ZLibWithAdler32DecompressAsync_InvalidChecksum_ShouldThrow()
    {
        // Arrange
        var originalData = "test"u8.ToArray();
        var compressedWithAdler = await originalData.ZLibWithAdler32CompressAsync();
        
        // Act & Assert
        compressedWithAdler[^1] ^= 0xFF;
        await Assert.ThrowsAsync<InvalidDataException>(compressedWithAdler.ZLibWithAdler32DecompressAsync);
    }
    
    #endregion
    
    #region Zip

    public static TheoryData<byte[], string> ZipCompressTheories =>
        new()
        {
            { [0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00, 0x08, 0x00, 0xB7, 0x8A, 0xA2, 0x5A, 0xA3, 0x1C, 0x29, 0x1C, 0x0E, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x64, 0x61, 0x74, 0x61, 0xF3, 0x48, 0xCD, 0xC9, 0xC9, 0x57, 0x08, 0xCF, 0x2F, 0xCA, 0x49, 0x51, 0x04, 0x00, 0x50, 0x4B, 0x01, 0x02, 0x14, 0x00, 0x14, 0x00, 0x00, 0x00, 0x08, 0x00, 0xB7, 0x8A, 0xA2, 0x5A, 0xA3, 0x1C, 0x29, 0x1C, 0x0E, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x61, 0x74, 0x61, 0x50, 0x4B, 0x05, 0x06, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x32, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x00, 0x00], "Hello World!" },
            { [0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x00, 0x00, 0x08, 0x00, 0x57, 0x8D, 0xA2, 0x5A, 0x52, 0xC1, 0xB6, 0xA9, 0x1E, 0x00, 0x00, 0x00, 0x1B, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x64, 0x61, 0x74, 0x61, 0x8B, 0x4A, 0x3C, 0xBA, 0xE7, 0xF0, 0xE6, 0xA3, 0x4D, 0x47, 0xDA, 0x15, 0xD2, 0x8F, 0xCC, 0x3C, 0x3A, 0x3B, 0xE7, 0x48, 0xAB, 0x42, 0x56, 0xE2, 0xD1, 0x5D, 0x47, 0x5B, 0x14, 0x01, 0x50, 0x4B, 0x01, 0x02, 0x14, 0x00, 0x14, 0x00, 0x00, 0x00, 0x08, 0x00, 0x57, 0x8D, 0xA2, 0x5A, 0x52, 0xC1, 0xB6, 0xA9, 0x1E, 0x00, 0x00, 0x00, 0x1B, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x64, 0x61, 0x74, 0x61, 0x50, 0x4B, 0x05, 0x06, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x32, 0x00, 0x00, 0x00, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00], "Zażółć gęślą jaźń!" }
        };

    [Theory]
    [MemberData(nameof(ZipCompressTheories))]
    public void Zip_ZipCompress_ShouldDecompressCorrectly(byte[] inputByteArray, string expectedString)
    {
        // Arrange
        var expectedBytes = Encoding.UTF8.GetBytes(expectedString);

        // Act
        var decompressedData = inputByteArray.ZipDecompress();
        
        // Assert
        Assert.Equal(expectedBytes, decompressedData);
    }

    [Theory]
    [MemberData(nameof(ZipCompressTheories))]
    public async Task Zip_ZipCompressAsync_ShouldDecompressCorrectly(byte[] inputByteArray, string expectedString)
    {
        // Arrange
        var expectedBytes = Encoding.UTF8.GetBytes(expectedString);

        // Act
        var decompressedData = await inputByteArray.ZipDecompressAsync();

        // Assert
        Assert.Equal(expectedBytes, decompressedData);
    }

    [Theory]
    [MemberData(nameof(CompressTheories))]
    public void Zip_ZipCompress_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = originalData.ZipCompress();
        var decompressedData = compressedData.ZipDecompress();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }
    
    [Theory]
    [MemberData(nameof(CompressTheories))]
    public async Task Zip_ZipCompressAsync_ShouldCompressAndDecompressCorrectly(string inputString)
    {
        // Arrange
        var originalData = Encoding.UTF8.GetBytes(inputString);

        // Act
        var compressedData = await originalData.ZipCompressAsync();
        var decompressedData = await compressedData.ZipDecompressAsync();

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(originalData, decompressedData);
    }
    
    [Fact]
    public void Zip_ZipCompressMultipleEntries_ShouldCompressAndDecompressCorrectly()
    {
        // Arrange
        const string entry1Name = "Hello World";
        const string entry2Name = "Zażółć gęślą jaźń";
        const string extension = ".txt";
        var entries = new List<ZipEntry>
        {
            new(Encoding.UTF8.GetBytes(entry1Name), $"{entry1Name}{extension}"),
            new(Encoding.UTF8.GetBytes(entry2Name), $"{entry2Name}{extension}")
        };

        // Act
        var compressedData = entries.ZipCompress();
        var decompressedData1 = compressedData.ZipDecompress($"{entry1Name}{extension}");
        var decompressedData2 = compressedData.ZipDecompress($"{entry2Name}{extension}");

        // Assert
        Assert.NotNull(compressedData);
        Assert.NotEmpty(compressedData);
        Assert.Equal(entries[0].EntryData, decompressedData1);
        Assert.Equal(entries[1].EntryData, decompressedData2);
    }

    #endregion
}