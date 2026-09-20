using Mi5hmasH.AesCrypto.Helpers;
using Mi5hmasH.AesCrypto.Interfaces;

namespace Mi5hmasH.AesCrypto.Flavors.Cbc;

/// <summary>
/// Provides AES-256 CBC encryption and decryption backed by hardware-accelerated AES-NI instructions.
/// This implementation uses the CPU's native AES-NI instruction set for optimized block cipher
/// operations and is selected when the platform supports it, otherwise a managed fallback backend is used.
/// </summary>
/// <remarks>
/// The backend pre-computes the forward round-key schedule during construction and, when configured as a decryptor,
/// additionally computes the inverted key schedule for efficient decryption. Key material can be replaced at any time
/// by calling <see cref="SetKey"/>. The initialization vector (IV) can be updated independently via <see cref="SetIv"/>.
/// </remarks>
public sealed class AesCbc256NiBackend : IAesWithIvBackend
{
    private readonly bool _isDecryptor;
    private readonly byte[]? _encKeys;
    private readonly byte[]? _decKeys;
    private readonly byte[] _iv;

    /// <summary>
    /// Hardware-accelerated AES-256 CBC backend that leverages AES-NI instructions for encryption and decryption operations.
    /// </summary>
    /// <param name="key256">The 256-bit AES key used to generate the encryption and decryption round keys.</param>
    /// <param name="iv">The 128-bit initialization vector used for CBC chaining.</param>
    /// <param name="isDecryptor">
    /// Indicates whether the backend should prepare decryption round keys.
    /// </param>
    public AesCbc256NiBackend(ReadOnlySpan<byte> key256, ReadOnlySpan<byte> iv, bool isDecryptor)
    {
        _isDecryptor = isDecryptor;
        _encKeys = GC.AllocateUninitializedArray<byte>(AesHelper.Aes256RoundKeyBufferLength);
        _iv = iv.ToArray();
        key256.ExpandKey256(_encKeys);
        if (!_isDecryptor) return;
        _decKeys = GC.AllocateUninitializedArray<byte>(AesHelper.Aes256RoundKeyBufferLength);
        _encKeys.InvertKeySchedule(_decKeys);
    }

    /// <summary>
    /// Replaces the current AES-256 key material and re-derives the forward and inverted round-key schedules.
    /// </summary>
    /// <param name="key256">A read-only span containing the 256-bit (32-byte) AES key.</param>
    public void SetKey(ReadOnlySpan<byte> key256)
    {
        key256.ExpandKey256(_encKeys);
        if (!_isDecryptor) return;
        _encKeys.InvertKeySchedule(_decKeys);
    }

    /// <summary>
    /// Replaces the current initialization vector used by CBC mode.
    /// </summary>
    /// <param name="iv">
    /// A 128-bit (16-byte) initialization vector.
    /// </param>
    public void SetIv(ReadOnlySpan<byte> iv) => iv.CopyTo(_iv);

    /// <summary>
    /// Encrypts the specified plaintext data using AES-256 in CBC mode via AES-NI hardware instructions.
    /// </summary>
    /// <param name="data">The plaintext byte sequence to encrypt. The length must be a multiple of 16 bytes.</param>
    /// <returns> A newly allocated byte array containing the ciphertext.</returns>
    public byte[] Encrypt(ReadOnlySpan<byte> data)
    {
        var output = GC.AllocateUninitializedArray<byte>(data.Length);
        AesCbc256Ni.Encrypt(_encKeys, _iv, data, output);
        return output;
    }

    /// <summary>
    /// Decrypts the specified ciphertext using AES-256 in CBC mode via AES-NI hardware instructions.
    /// </summary>
    /// <param name="data"> The ciphertext byte sequence to decrypt. The length must be a multiple of 16 bytes.</param>
    /// <returns>A newly allocated byte array containing the decrypted plaintext.</returns>
    public byte[] Decrypt(ReadOnlySpan<byte> data)
    {
        var output = GC.AllocateUninitializedArray<byte>(data.Length);
        AesCbc256Ni.Decrypt(_decKeys, _iv, data, output);
        return output;
    }
}