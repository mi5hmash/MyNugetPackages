using System.Diagnostics;
using System.Security.Cryptography;

namespace Mi5hmasH.AesCrypto.Helpers;

public static class CryptoHelper
{
    public const byte IvLength = 16;
    public const byte SignatureLength = 16;
    
    /// <summary>
    /// Generates a cryptographically random salt of the specified length.
    /// </summary>
    /// <param name="saltLength">The desired length of the salt in bytes.</param>
    /// <return>A byte array containing the randomly generated salt.</return>
    public static byte[] GenerateSalt(int saltLength)
    {
        Random random = new();
        var salt = new byte[saltLength];
        for (var i = 0; i < salt.Length; i++) 
            salt[i] = (byte)random.Next(byte.MaxValue + 1);
        return salt;
    }

    /// <summary>
    /// Computes the MD5 checksum of the provided byte sequence.
    /// </summary>
    /// <param name="bytes">The read-only span of bytes over which the checksum is calculated.</param>
    /// <return>A byte array containing the 128-bit MD5 hash of the input data.</return>
    public static byte[] ComputeChecksum(ReadOnlySpan<byte> bytes) => MD5.HashData(bytes);

    /// <summary>
    /// Validates the integrity of a byte sequence by comparing its trailing signature against a freshly computed MD5 checksum of the data portion.
    /// </summary>
    /// <param name="bytes">A read-only span of bytes where the last 16 bytes represent the expected MD5 signature and the preceding bytes represent the data to be verified.</param>
    /// <return><see langword="true"/> if the computed checksum matches the embedded signature; otherwise, <see langword="false"/>.</return>
    public static bool IsSignatureValid(ReadOnlySpan<byte> bytes)
    {
        var dataLength = bytes.Length - SignatureLength;
        var signature = bytes[dataLength..];
        var newChecksum = ComputeChecksum(bytes[..dataLength]);
        return signature.SequenceEqual(newChecksum);
    }
    
    /// <summary>
    /// Creates and configures an AES instance with the specified key for CBC-mode encryption or decryption.
    /// </summary>
    /// <param name="key">The secret key bytes used to initialize the AES cipher.</param>
    /// <return>A configured <see cref="System.Runtime.Intrinsics.X86.Aes"/> instance set to CBC mode with no padding and the supplied key.</return>
    public static Aes GetAes(ReadOnlySpan<byte> key)
    {
        var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.None;
        aes.Key = key.ToArray();
        return aes;
    }

#if DEBUG
    /// <summary>
    /// Generates a random 32-byte key, encodes it in Base64, prints it to the debug output, and returns the Base64 string. 
    /// This method is intended for debugging purposes only and should not be used in production code.
    /// </summary>
    /// <returns>A Base64-encoded string representing the randomly generated 32-byte key.</returns>
    public static string Debug_GenerateBase64Key()
    {
        var result = Convert.ToBase64String(GenerateSalt(32));
        Debug.Print(result);
        return result;
    }

    /// <summary>
    /// Generates a random 32-byte key, prints it in a C# byte array format to the debug output, and returns the byte array. 
    /// This method is intended for debugging purposes only and should not be used in production code.
    /// </summary>
    /// <returns>A byte array representing the randomly generated 32-byte key.</returns>
    public static string Debug_GenerateKey()
    {
        var key = GenerateSalt(32);
        var result =
            "byte[] key =\n[\n    " +
            string.Join(", ", key.Select(b => $"0x{b:X2}")) +
            "\n];";
        
        Debug.Print(result);
        return result;
    }
#endif
}