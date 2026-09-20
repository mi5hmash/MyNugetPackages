using System.Security.Cryptography;
using Mi5hmasH.AesCrypto.Interfaces;

namespace Mi5hmasH.AesCrypto.Flavors.Ecb;

public sealed class AesEcb256ManagedBackend(Aes aes) : IAesBackend, IDisposable
{
    /// <summary>
    /// The underlying AES algorithm instance used to perform all symmetric encryption and decryption operations.
    /// Injected via the constructor and owned by this backend for its lifetime.
    /// </summary>
    private readonly Aes _aes = aes;

    /// <summary>
    /// Sets the AES-256 key used for subsequent encryption or decryption operations.
    /// </summary>
    /// <param name="key256">A read-only span containing the 256-bit (32-byte) AES key to be assigned to the internal AES instance.</param>
    public void SetKey(ReadOnlySpan<byte> key256)
        => _aes.Key = key256.ToArray();
    
    /// <summary>
    /// Encrypts a block of plaintext data using AES-256 in ECB mode with the configured padding scheme.
    /// </summary>
    /// <param name="plainBytesWithPadding">A read-only span of plaintext bytes that must be padded to a block-size boundary prior to encryption.</param>
    /// <returns>A newly allocated byte array containing the ciphertext of the same length as the input span.</returns>
    public byte[] Encrypt(ReadOnlySpan<byte> plainBytesWithPadding)
    {
        var encrypted = GC.AllocateUninitializedArray<byte>(plainBytesWithPadding.Length);
        _aes.EncryptEcb(plainBytesWithPadding, encrypted, _aes.Padding);
        return encrypted;
    }

    /// <summary>
    /// Decrypts the specified AES-256 ECB-encrypted data using the configured AES instance and padding mode, returning the resulting plaintext bytes.
    /// </summary>
    /// <param name="encryptedBytes">A read-only span containing the ciphertext bytes to be decrypted.</param>
    /// <returns>A byte array containing the decrypted plaintext, allocated with the same length as the input.</returns>
    public byte[] Decrypt(ReadOnlySpan<byte> encryptedBytes)
    {
        var decrypted = GC.AllocateUninitializedArray<byte>(encryptedBytes.Length);
        _aes.DecryptEcb(encryptedBytes, decrypted, _aes.Padding);
        return decrypted;
    }
    
    /// <summary>
    /// Releases the unmanaged resources used by the underlying <see cref="Aes"/> instance and frees all associated resources held by this backend.
    /// </summary>
    public void Dispose() => _aes.Dispose();
}
