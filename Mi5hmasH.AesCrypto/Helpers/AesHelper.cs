using System.Runtime.Intrinsics.X86;

namespace Mi5hmasH.AesCrypto.Helpers;

public class AesHelper
{
    /// <summary>
    /// The length in bytes of the expanded AES-256 round key buffer, equal to 15 round keys multiplied by 16 bytes each (240 bytes total).
    /// </summary>
    public const int Aes256RoundKeyBufferLength = 240;
    
    /// <summary>
    /// Determines whether the current processor supports AES-NI (AES New Instructions)
    /// by verifying that both the AES hardware instructions and SSE2 extensions are available.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the CPU supports AES-NI (both <see cref="System.Runtime.Intrinsics.X86.Aes"/> and
    /// <see cref="System.Runtime.Intrinsics.X86.Sse2"/> are available); otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsAesNiSupported() =>
        Aes.IsSupported && Sse2.IsSupported;
}