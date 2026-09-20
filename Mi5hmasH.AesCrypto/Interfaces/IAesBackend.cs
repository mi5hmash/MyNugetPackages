namespace Mi5hmasH.AesCrypto.Interfaces;

/// <summary>
/// Defines the contract for an AES cryptographic backend implementation.
/// Provides a uniform abstraction over encryption, decryption, and key management operations.
/// </summary>
/// <remarks>
/// Implementations of this interface are expected to conform to the AES algorithm specification.
/// The backend maintains internal state for the active key, which must be set before any cryptographic operation is performed.
/// </remarks>
public interface IAesBackend
{
    void SetKey(ReadOnlySpan<byte> key);
    byte[] Encrypt(ReadOnlySpan<byte> data);
    byte[] Decrypt(ReadOnlySpan<byte> data);
}