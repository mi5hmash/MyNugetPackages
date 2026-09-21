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
    public static bool IsAesNiSupported() 
        => Aes.IsSupported && Sse2.IsSupported;
    
    /// <summary>
    /// Determines the possible PKCS#7 padding length in the provided data.
    /// </summary>
    /// <param name="dataWithPadding">A read-only span of bytes representing the data to analyze.
    /// The data must be at least 16 bytes long to contain valid PKCS#7 padding.</param>
    /// <returns>The length of the PKCS#7 padding if valid padding is detected; otherwise, 0.</returns>
    public static byte GetPossiblePkcs7PaddingLength(ReadOnlySpan<byte> dataWithPadding)
    {
        if (dataWithPadding.Length < 16) return 0;
        var padLen = dataWithPadding[^1];
        if (padLen is > 16 or < 1) return 0;
        // Thank you Dxian998 for proposing a cleaner loop.
        for (var i = 1; i <= padLen; i++)
            if (dataWithPadding[^i] != padLen) return 0;

        return padLen;
    }

    /// <summary>
    /// Calculates the number of PKCS#7 padding bytes required to align the given data length
    /// to the AES block size of 16 bytes.
    /// </summary>
    /// <param name="dataWithNoPaddingLength">The length in bytes of the unpadded data for which
    /// padding must be determined.</param>
    /// <returns>
    /// The number of padding bytes (1 to 16) that need to be appended so that the total data length is a multiple
    /// of the 16-byte AES block size.
    /// </returns>
    public static byte CalculatePkcs7PaddingLength(int dataWithNoPaddingLength)
    {
        const int blockSize = 16;
        return (byte)(blockSize - dataWithNoPaddingLength % blockSize);
    }

    /// <summary>
    /// Calculates the number of PKCS#7 padding bytes required to align the provided data
    /// to the AES block size (16 bytes).
    /// </summary>
    /// <param name="dataWithNoPadding">
    /// A read-only span of bytes representing the un-padded plaintext data whose
    /// padding length is to be computed.
    /// </param>
    /// <returns>
    /// The number of padding bytes (1 to 16) that must be appended so that the total
    /// length of the data becomes a multiple of the 16-byte AES block size.
    /// </returns>
    public static byte CalculatePkcs7PaddingLength(ReadOnlySpan<byte> dataWithNoPadding)
        => CalculatePkcs7PaddingLength(dataWithNoPadding.Length);
    
    /// <summary>
    /// Removes PKCS#7 padding from the specified data.
    /// </summary>
    /// <param name="data">The input data from which the padding will be removed.</param>
    /// <returns>A new byte array containing the data with the padding removed.</returns>
    public static byte[] RemovePkcs7Padding(ReadOnlySpan<byte> data)
    {
        var padLen = GetPossiblePkcs7PaddingLength(data);
        return data[..^padLen].ToArray();
    }
    
    /// <summary>
    /// Adds PKCS#7 padding to the specified data to ensure its length is a multiple of the block size.
    /// </summary>
    /// <param name="data">The input data to which padding will be added.</param>
    /// <returns>A new byte array containing the original data followed by PKCS#7 padding bytes.</returns>
    public static byte[] AddPkcs7Padding(ReadOnlySpan<byte> data)
    {
        var padLen = CalculatePkcs7PaddingLength(data);
        var newDataLength = data.Length + padLen;
        var dataWithPadding = GC.AllocateUninitializedArray<byte>(newDataLength);
        var dataWithPaddingAsSpan = dataWithPadding.AsSpan();
        data.CopyTo(dataWithPaddingAsSpan);
        AddPkcs7PaddingInPlace(dataWithPaddingAsSpan, padLen);
        
        return dataWithPadding;
    }

    /// <summary>
    /// Applies PKCS#7 padding to the last bytes of the provided data in place by filling them with
    /// the specified padding length value.
    /// </summary>
    /// <param name="data">The span of bytes representing the data buffer to pad. The padding is written
    /// into the trailing positions of this span and must have sufficient space for the desired block alignment.</param>
    /// <param name="paddingLength">The number of padding bytes to apply, corresponding to the number of bytes
    /// needed to reach the next AES block boundary (16 bytes). Each of the trailing bytes is set to this value.</param>
    public static void AddPkcs7PaddingInPlace(Span<byte> data, byte paddingLength)
    {
        for (var i = 1; i < paddingLength + 1; i++)
            data[^i] = paddingLength;
    }
}