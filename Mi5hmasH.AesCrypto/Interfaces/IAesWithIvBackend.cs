namespace Mi5hmasH.AesCrypto.Interfaces;

/// <summary>
/// Defines a contract for an AES encryption backend that supports explicit Initialization Vector (IV) management.
/// Implementations of this interface provide stateful encryption and decryption operations where both
/// the symmetric key and the IV must be configured prior to processing data.
/// </summary>
public interface IAesWithIvBackend
{
    void SetIv(ReadOnlySpan<byte> iv);
    void SetKey(ReadOnlySpan<byte> key);
    byte[] Encrypt(ReadOnlySpan<byte> data);
    byte[] Decrypt(ReadOnlySpan<byte> data);
}