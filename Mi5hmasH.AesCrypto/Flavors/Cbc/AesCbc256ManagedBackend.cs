using System.Security.Cryptography;
using Mi5hmasH.AesCrypto.Interfaces;

namespace Mi5hmasH.AesCrypto.Flavors.Cbc;

public sealed class AesCbc256ManagedBackend(Aes aes) : IAesWithIvBackend, IDisposable
{
    /// <summary>
    /// The underlying AES algorithm instance used to perform all symmetric encryption and decryption operations.
    /// Injected via the constructor and owned by this backend for its lifetime.
    /// </summary>
    private readonly Aes _aes = aes;

    /// <summary>
    /// Sets the AES-256 key used for subsequent encryption and decryption operations.
    /// </summary>
    public void SetKey(ReadOnlySpan<byte> key256)
        => _aes.Key = key256.ToArray();

    /// <summary>
    /// Sets the 128-bit (16-byte) initialization vector used for CBC mode.
    /// </summary>
    public void SetIv(ReadOnlySpan<byte> iv)
        => _aes.IV = iv.ToArray();

    /// <summary>
    /// Encrypts plaintext using AES-256 CBC mode.
    /// </summary>
    /// <param name="plainBytesWithPadding">Plaintext bytes that are already padded as required.</param>
    /// <returns>The encrypted ciphertext.</returns>
    public byte[] Encrypt(ReadOnlySpan<byte> plainBytesWithPadding)
    {
        var encrypted = GC.AllocateUninitializedArray<byte>(plainBytesWithPadding.Length);
        _aes.EncryptCbc(plainBytesWithPadding, _aes.IV, encrypted, _aes.Padding);
        return encrypted;
    }

    /// <summary>
    /// Decrypts AES-256 CBC ciphertext.
    /// </summary>
    /// <param name="encryptedBytes">Ciphertext bytes to decrypt.</param>
    /// <returns>The decrypted plaintext.</returns>
    public byte[] Decrypt(ReadOnlySpan<byte> encryptedBytes)
    {
        var decrypted = GC.AllocateUninitializedArray<byte>(encryptedBytes.Length);
        _aes.DecryptCbc(encryptedBytes, _aes.IV, decrypted, _aes.Padding);
        return decrypted;
    }

    /// <summary>
    /// Releases all resources used by the underlying AES instance.
    /// </summary>
    public void Dispose() => _aes.Dispose();
}
