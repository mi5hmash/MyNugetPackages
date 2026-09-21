using System.Security.Cryptography;
using System.Text;
using Mi5hmasH.AesCrypto.Flavors.Cbc;
using Mi5hmasH.AesCrypto.Helpers;
using Mi5hmasH.AesCrypto.Interfaces;
using Mi5hmasH.Compressors;
using static Mi5hmasH.AesCrypto.Helpers.CryptoHelper;

namespace Mi5hmasH.AesCrypto;

public class Crypto(ReadOnlySpan<byte> key, bool isDecryptor, Encoding? textEncoding = null) : IDisposable
{
    private const int SaltLength = 64;

    /// <summary>
    /// The text encoding used to convert between strings and byte arrays during encryption and decryption.
    /// Defaults to UTF-8 when no encoding is explicitly provided at construction time.
    /// </summary>
    private readonly Encoding _textEncoding = textEncoding ?? Encoding.UTF8;

    /// <summary>
    /// The AES-256-CBC encryption backend instance used for performing cryptographic operations.
    /// Automatically selects a hardware-accelerated implementation (AES-NI) when supported by the current CPU,
    /// otherwise falls back to the managed software implementation.
    /// </summary>
    private readonly IAesWithIvBackend _backend = AesHelper.IsAesNiSupported() 
        ? new AesCbc256NiBackend(key, GC.AllocateUninitializedArray<byte>(16), isDecryptor)
        : new AesCbc256ManagedBackend(GetAes(key));

    /// <summary>
    /// Sets the encryption key to be used for subsequent AES-256-CBC encryption and decryption operations.
    /// </summary>
    /// <param name="keySpan">A read-only span containing the bytes of the encryption key to apply.</param>
    public void SetKey(ReadOnlySpan<byte> keySpan)
        => _backend.SetKey(keySpan);

    /// <summary>
    /// Sets the initialization vector (IV) to be used for subsequent AES-256-CBC encryption and decryption operations.
    /// </summary>
    /// <param name="iv">A read-only span containing the bytes of the initialization vector to apply.</param>
    public void SetIv(ReadOnlySpan<byte> iv) => _backend.SetIv(iv);

    /// <summary>
    /// Encrypts the specified plain text string using the configured encryption key and encoding, then returns the result as a Base64-encoded string.
    /// </summary>
    /// <param name="text">The plain text string to be encrypted.</param>
    /// <return>A Base64-encoded string containing the encrypted data.</return>
    public string Encrypt(string text)
    {
        var byteArray = _textEncoding.GetBytes(text);
        var encryptedBytes = Encrypt(byteArray);
        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Encrypts the specified data using AES-CBC-256 with a randomly generated initialization vector.
    /// The method optionally compresses the input data using Brotli before encryption if compression is beneficial.
    /// The resulting byte array is a structured container that includes the initialization vector, a compression flag,
    /// the encrypted payload, a random salt, and an MD5 checksum for integrity verification.
    /// </summary>
    /// <param name="data">The plaintext data to be encrypted.</param>
    /// <return>
    /// A byte array containing the complete encrypted container with the initialization vector, compression flag,
    /// ciphertext, salt, and integrity signature.
    /// </return>
    public byte[] Encrypt(ReadOnlySpan<byte> data)
    {
        // Configure IV
        var iv = GenerateSalt(IvLength);
        SetIv(iv);
        
        // Compress data only if beneficial
        var compressedData = data.ToArray().BrotliCompress();
        var useCompressed = compressedData.Length < data.Length;

        // Calculate padding length
        var dataToEncrypt = useCompressed 
            ? compressedData.AsSpan()
            : data;
        var paddingLength = AesHelper.CalculatePkcs7PaddingLength(1 + dataToEncrypt.Length);
        
        // Create data container
        var dataContainerLength = IvLength + 1 + dataToEncrypt.Length + paddingLength + SaltLength + SignatureLength;
        var dataContainer = GC.AllocateUninitializedArray<byte>(dataContainerLength);
        var dataContainerSpan = dataContainer.AsSpan();
        
        // Write IV
        iv.CopyTo(dataContainerSpan);

        // Write Compression Flag
        dataContainerSpan[IvLength] = useCompressed
            ? (byte)0x01
            : (byte)0x00;

        // Write data
        dataToEncrypt.CopyTo(dataContainerSpan[(IvLength + 1)..]);

        // Add padding
        var dataToEncryptSpan = dataContainerSpan.Slice(IvLength, 1 + dataToEncrypt.Length + paddingLength);
        AesHelper.AddPkcs7PaddingInPlace(dataToEncryptSpan, paddingLength);

        // Encrypt data
        var encryptedData = _backend.Encrypt(dataToEncryptSpan);

        // Write encrypted data
        encryptedData.CopyTo(dataContainerSpan[IvLength..]);

        // Write salt
        var salt = GenerateSalt(SaltLength);
        salt.CopyTo(dataContainerSpan[^(SaltLength + SignatureLength)..]);

        // Compute and write checksum
        var signature = ComputeChecksum(dataContainerSpan[..^SignatureLength]);
        signature.CopyTo(dataContainerSpan[^SignatureLength..]);

        return dataContainer;
    }

    /// <summary>
    /// Decrypts a Base64-encoded encrypted string and returns the plaintext representation using the configured text encoding.
    /// </summary>
    /// <param name="text">The Base64-encoded string containing the encrypted data to be decrypted.</param>
    /// <return>A string containing the decrypted plaintext content.</return>
    public string Decrypt(string text)
    {
        var bytes = Convert.FromBase64String(text);
        var decryptedBytes = Decrypt(bytes);
        return _textEncoding.GetString(decryptedBytes);
    }

    /// <summary>
    /// Decrypts a raw binary payload that was previously encrypted and packaged with an AES-256-CBC scheme.
    /// The input is expected to contain a leading IV, the encrypted content, a trailing salt section, and a signature block.
    /// The method validates the integrity signature, extracts and applies the initialization vector, performs AES decryption,
    /// and optionally decompresses the resulting data using Brotli if the compression flag is set.
    /// </summary>
    /// <param name="data">A read-only byte span containing the full encrypted container, including IV prefix, ciphertext, salt, and signature suffix.</param>
    /// <returns>A byte array containing the decrypted plaintext, decompressed via Brotli if the original data was compressed.</returns>
    /// <exception cref="CryptographicException">Thrown when the integrity signature of the input data is invalid or when the decrypted container is unexpectedly empty.</exception>
    public byte[] Decrypt(ReadOnlySpan<byte> data)
    {
        // Validate signature early
        if (!IsSignatureValid(data)) throw new CryptographicException("Invalid signature");

        // Extract IV and update it in AES backend
        SetIv(data[..IvLength]);

        // Determine data length and slice relevant portion
        var dataLength = data.Length - IvLength - SaltLength - SignatureLength;
        var dataSpan = data.Slice(IvLength, dataLength);

        // Decrypt container
        var decryptedContainer = _backend.Decrypt(dataSpan);
        // Calculate Padding
        var paddingLength = AesHelper.GetPossiblePkcs7PaddingLength(decryptedContainer);
        var decryptedContainerSpan = decryptedContainer.AsSpan()[..^paddingLength];

        if (decryptedContainerSpan.Length == 0) throw new CryptographicException("Container should not be empty");

        // Check compression flag
        var isDataCompressed = decryptedContainer[0] == 0x01;
        var decryptedData = decryptedContainerSpan[1..].ToArray();

        // Decompress if needed
        return isDataCompressed ? decryptedData.BrotliDecompress() : decryptedData;
    }
    
    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="Crypto"/> instance and disposes of the internal AES backend.
    /// </summary>
    public void Dispose()
    {
        if (_backend is IDisposable d)
            d.Dispose();
        
        GC.SuppressFinalize(this);
    }
}