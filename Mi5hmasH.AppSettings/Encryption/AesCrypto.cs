using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Mi5hmasH.AppSettings.Encryption;

public class AesCrypto
{
    private byte[] _key = null!;

    private const byte SignatureLength = 16;
    private const byte IvLength = 16;

    /// <summary>
    /// The encryption key.
    /// </summary>
    public string Key
    {
        set => _key = Convert.FromBase64String(value);
    }

    /// <summary>
    /// Length of the salt in bytes.
    /// </summary>
    public uint SaltLength { private get; set; }

    /// <summary>
    /// Encoding used for text conversion.
    /// </summary>
    public Encoding TextEncoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Creates a new instance of the AesCrypto class.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="saltLength"></param>
    public AesCrypto(string key, uint saltLength = 64)
    {
        Key = key;
        SaltLength = saltLength;
    }
    
    /// <summary>
    /// Generates a random salt of the specified length.
    /// </summary>
    /// <param name="saltLength"></param>
    /// <returns></returns>
    public static byte[] GenerateSalt(uint saltLength)
    {
        Random random = new();
        var salt = new byte[saltLength];
        for (var i = 0; i < salt.Length; i++) 
            salt[i] = (byte)random.Next(byte.MaxValue + 1);
        return salt;
    }

    /// <summary>
    /// Computes the md5 checksum for the specified byte array.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private static byte[] ComputeChecksum(ReadOnlySpan<byte> bytes)
        => MD5.HashData(bytes);

    /// <summary>
    /// Validates signature.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    private static bool IsSignatureValid(ReadOnlySpan<byte> bytes)
    {
        var dataLength = bytes.Length - SignatureLength;
        var signature = bytes[dataLength..];
        var newChecksum = ComputeChecksum(bytes[..dataLength]);
        return signature.SequenceEqual(newChecksum);
    }

    /// <summary>
    /// Gets the AES object with CBC mode and PKCS7 padding.
    /// </summary>
    /// <returns></returns>
    private static Aes GetAes()
    {
        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes;
    }

    /// <summary>
    /// Encrypts the specified string.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string Encrypt(string text)
    {
        var byteArray = TextEncoding.GetBytes(text);
        var encryptedBytes = Encrypt(byteArray);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Encrypts the specified byte array.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public byte[] Encrypt(byte[] bytes)
    {
        // Initialize AES
        using var aes = GetAes();
        aes.Key = _key;
        aes.GenerateIV();

        using MemoryStream ms = new();
        ms.Write(aes.IV); // Write IV to data stream

        // Compress only if beneficial
        var compressedBytes = BrotliCompress(bytes);
        var useCompressed = compressedBytes.Length < bytes.Length;

        using MemoryStream ms2 = new();
        ms2.WriteByte(useCompressed ? (byte)0x01 : (byte)0x00);
        ms2.Write(useCompressed ? compressedBytes : bytes);

        var dataToWrite = ms2.ToArray();

        // Encrypt in a single operation
        using var encryptor = aes.CreateEncryptor();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write, true);
        cs.Write(dataToWrite, 0, dataToWrite.Length);
        cs.FlushFinalBlock();

        // Append salt
        var salt = GenerateSalt(SaltLength);
        ms.Write(salt);

        // Append checksum
        var signature = ComputeChecksum(ms.ToArray());
        ms.Write(signature);

        return ms.ToArray();
    }

    /// <summary>
    /// Decrypts the specified string.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public string Decrypt(string text)
    {
        var bytes = Convert.FromBase64String(text);
        var decryptedBytes = Decrypt(bytes);
        return TextEncoding.GetString(decryptedBytes);
    }

    /// <summary>
    /// Decrypts the specified byte array.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public byte[] Decrypt(byte[] bytes)
    {
        ReadOnlySpan<byte> spanByte = bytes;

        // Validate signature early
        if (!IsSignatureValid(spanByte)) throw new Exception("Invalid signature");

        // Extract IV and initialize AES
        using var aes = GetAes();
        aes.Key = _key;
        aes.IV = spanByte[..IvLength].ToArray();

        // Determine data length and slice relevant portion
        var dataLength = spanByte.Length - IvLength - SaltLength - SignatureLength;
        var dataSpan = spanByte.Slice(IvLength, (int)dataLength);

        using MemoryStream msi = new(dataSpan.ToArray());
        using var decryptor = aes.CreateDecryptor();
        using CryptoStream cs = new(msi, decryptor, CryptoStreamMode.Read);
        using MemoryStream mso = new();
        cs.CopyTo(mso);

        // Ensure valid decrypted container
        var decryptedContainer = mso.ToArray();
        if (decryptedContainer.Length == 0) throw new Exception("Container should not be empty");

        // Determine compression flag
        var isDataCompressed = decryptedContainer[0] == 0x01;
        var decryptedData = decryptedContainer.AsSpan(1).ToArray();

        // Decompress if needed
        return isDataCompressed ? BrotliDecompress(decryptedData) : decryptedData;
    }

    #region COMPRESSION

    /// <summary>
    /// Compresses <paramref name="bytes"/> with Brotli.
    /// </summary>
    /// <param name="bytes">The byte array to be compressed.</param>
    /// <param name="compressionLevel">The level of compression to apply (default: Optimal).</param>
    /// <returns>The compressed byte array.</returns>
    private static byte[] BrotliCompress(byte[] bytes, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        using var mso = new MemoryStream();
        using (var bs = new BrotliStream(mso, compressionLevel)) 
            bs.Write(bytes, 0, bytes.Length);
        return mso.ToArray();
    }

    /// <summary>
    /// Decompresses <paramref name="bytes"/> with Brotli.
    /// </summary>
    /// <param name="bytes">The Brotli-compressed byte array to be decompressed.</param>
    /// <returns>The decompressed byte array.</returns>
    private static byte[] BrotliDecompress(byte[] bytes)
    {
        using var msi = new MemoryStream(bytes);
        using var mso = new MemoryStream();
        using (var bs = new BrotliStream(msi, CompressionMode.Decompress)) 
            bs.CopyTo(mso);
        return mso.ToArray();
    }

    #endregion

#if DEBUG
    /// <summary>
    /// Generates a random key for debugging purposes.
    /// </summary>
    public static string Debug_GenerateKey()
    {
        var result = Convert.ToBase64String(GenerateSalt(32));
        Debug.Print(result);
        return result;
    }
#endif
}