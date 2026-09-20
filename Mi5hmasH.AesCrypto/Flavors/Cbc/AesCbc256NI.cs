using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Mi5hmasH.AesCrypto.Flavors.Ecb;

namespace Mi5hmasH.AesCrypto.Flavors.Cbc;

public static class AesCbc256Ni
{
    /// <summary>
    /// Encrypts the plaintext buffer using AES-256 in CBC mode with hardware-accelerated AES-NI instructions.
    /// Each 16-byte block is XORed with the previous ciphertext block (or the IV for the first block)
    /// before being encrypted with the expanded round-key schedule.
    /// </summary>
    /// <param name="roundKeyBuffer">The expanded AES-256 round-key buffer containing 15 round keys (240 bytes total), produced by <see cref="ExpandKey256"/>.</param>
    /// <param name="iv">The 16-byte initialization vector used to seed the CBC chain for the first block.</param>
    /// <param name="plaintext">The plaintext data to encrypt. Length must be a multiple of 16 bytes.</param>
    /// <param name="ciphertext">The destination span that receives the encrypted output. Length must be at least equal to <paramref name="plaintext"/> and a multiple of 16 bytes.</param>
    public static void Encrypt(
        ReadOnlySpan<byte> roundKeyBuffer,
        ReadOnlySpan<byte> iv,
        ReadOnlySpan<byte> plaintext,
        Span<byte> ciphertext)
    {
        var roundKeys = MemoryMarshal.Cast<byte, Vector128<byte>>(roundKeyBuffer);
        var inBlocks = MemoryMarshal.Cast<byte, Vector128<byte>>(plaintext);
        var outBlocks = MemoryMarshal.Cast<byte, Vector128<byte>>(ciphertext);
        var previous = MemoryMarshal.Read<Vector128<byte>>(iv);

        for (var i = 0; i < inBlocks.Length; i++)
        {
            var block = Sse2.Xor(inBlocks[i], previous);
            block = EncryptBlock(block, roundKeys);
            outBlocks[i] = block;
            previous = block;
        }
    }

    /// <summary>
    /// Decrypts a ciphertext buffer using AES-256-CBC mode via hardware-accelerated AES-NI and SSE2 instructions.
    /// Each ciphertext block is decrypted with the inverted key schedule, then XORed with the preceding ciphertext block
    /// (or the initialization vector for the first block) to produce the corresponding plaintext block.
    /// </summary>
    /// <param name="invertedRoundKeyBuffer">
    /// A precomputed buffer of 240 bytes containing the inverted AES-256 round keys suitable for decryption.
    /// Must be produced by <see cref="InvertKeySchedule"/> and aligned to 16-byte block boundaries.
    /// </param>
    /// <param name="iv">
    /// A 16-byte initialization vector used as the XOR operand for the first plaintext block.
    /// </param>
    /// <param name="ciphertext">
    /// The input ciphertext to be decrypted. Must be a multiple of 16 bytes in length.
    /// </param>
    /// <param name="plaintext">
    /// The destination buffer that receives the decrypted plaintext. Must be at least as long as <paramref name="ciphertext"/>
    /// and aligned to 16-byte block boundaries.
    /// </param>
    public static void Decrypt(
        ReadOnlySpan<byte> invertedRoundKeyBuffer,
        ReadOnlySpan<byte> iv,
        ReadOnlySpan<byte> ciphertext,
        Span<byte> plaintext)
    {
        var roundKeys = MemoryMarshal.Cast<byte, Vector128<byte>>(invertedRoundKeyBuffer);
        var inBlocks = MemoryMarshal.Cast<byte, Vector128<byte>>(ciphertext);
        var outBlocks = MemoryMarshal.Cast<byte, Vector128<byte>>(plaintext);
        var previous = MemoryMarshal.Read<Vector128<byte>>(iv);

        for (var i = 0; i < inBlocks.Length; i++)
        {
            var currentCipher = inBlocks[i];
            var block = DecryptBlock(currentCipher, roundKeys);
            block = Sse2.Xor(block, previous);

            outBlocks[i] = block;
            previous = currentCipher;
        }
    }

    /// <summary>
    /// Encrypts a single 128-bit block using the AES-256 cipher via AES-NI hardware intrinsics.
    /// Applies an initial AddRoundKey step, thirteen AES encryption rounds, and a final AES encryption round.
    /// </summary>
    /// <param name="block">The 128-bit plaintext block to encrypt.</param>
    /// <param name="roundKeys">A span containing the 15 AES-256 round key vectors used for the key schedule.</param>
    /// <return>The 128-bit ciphertext block resulting from the AES-256 block encryption.</return>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<byte> EncryptBlock(
        Vector128<byte> block,
        ReadOnlySpan<Vector128<byte>> roundKeys)
    {
        var state = Sse2.Xor(block, roundKeys[0]);

        state = Aes.Encrypt(state, roundKeys[1]);
        state = Aes.Encrypt(state, roundKeys[2]);
        state = Aes.Encrypt(state, roundKeys[3]);
        state = Aes.Encrypt(state, roundKeys[4]);
        state = Aes.Encrypt(state, roundKeys[5]);
        state = Aes.Encrypt(state, roundKeys[6]);
        state = Aes.Encrypt(state, roundKeys[7]);
        state = Aes.Encrypt(state, roundKeys[8]);
        state = Aes.Encrypt(state, roundKeys[9]);
        state = Aes.Encrypt(state, roundKeys[10]);
        state = Aes.Encrypt(state, roundKeys[11]);
        state = Aes.Encrypt(state, roundKeys[12]);
        state = Aes.Encrypt(state, roundKeys[13]);

        return Aes.EncryptLast(state, roundKeys[14]);
    }

    /// <summary>
    /// Decrypts a single 128-bit block using AES-NI hardware instructions by applying the full AES-256
    /// decryption key schedule: an initial round-key XOR followed by 13 intermediate decryption rounds
    /// and a final decryption round.
    /// </summary>
    /// <param name="block">The 128-bit block of ciphertext to be decrypted.</param>
    /// <param name="roundKeys">
    /// The 15 AES-256 round keys (indices 0 through 14) used to derive the decryption state.
    /// Must contain at least 15 elements.
    /// </param>
    /// <return>The decrypted 128-bit block.</return>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector128<byte> DecryptBlock(
        Vector128<byte> block,
        ReadOnlySpan<Vector128<byte>> roundKeys)
    {
        var state = Sse2.Xor(block, roundKeys[0]);

        state = Aes.Decrypt(state, roundKeys[1]);
        state = Aes.Decrypt(state, roundKeys[2]);
        state = Aes.Decrypt(state, roundKeys[3]);
        state = Aes.Decrypt(state, roundKeys[4]);
        state = Aes.Decrypt(state, roundKeys[5]);
        state = Aes.Decrypt(state, roundKeys[6]);
        state = Aes.Decrypt(state, roundKeys[7]);
        state = Aes.Decrypt(state, roundKeys[8]);
        state = Aes.Decrypt(state, roundKeys[9]);
        state = Aes.Decrypt(state, roundKeys[10]);
        state = Aes.Decrypt(state, roundKeys[11]);
        state = Aes.Decrypt(state, roundKeys[12]);
        state = Aes.Decrypt(state, roundKeys[13]);

        return Aes.DecryptLast(state, roundKeys[14]);
    }

    /// <summary>
    /// Expands a 256-bit AES key into a full round-key schedule suitable for use with hardware-accelerated AES-NI operations.
    /// The generated schedule is stored in the provided buffer and can be consumed by encryption or decryption routines.
    /// </summary>
    /// <param name="key256">The 32-byte (256-bit) symmetric key to expand.</param>
    /// <param name="roundKeyBuffer">The destination span that receives the expanded round-key schedule. Must be at least 240 bytes to accommodate all 15 round keys.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ExpandKey256(this ReadOnlySpan<byte> key256, Span<byte> roundKeyBuffer) 
        => AesEcb256Ni.ExpandKey256(key256, roundKeyBuffer);

    /// <summary>
    /// Inverts the AES-256 encryption key schedule to produce a key schedule suitable for decryption operations.
    /// The AES-256 key schedule consists of 15 round keys (240 bytes total). Decryption requires the round keys
    /// to be processed in reverse order, with the intermediate keys transformed using the inverse MixColumns
    /// operation. This method performs that transformation in-place, writing the result into the output buffer.
    /// The output must be passed to a decryption routine such as <see cref="AesCbc256Ni.Decrypt"/>
    /// to correctly recover plaintext from ciphertext.
    /// </summary>
    /// <param name="roundKeys">
    /// A 240-byte buffer containing the original AES-256 encryption round keys, where each 16-byte segment
    /// corresponds to one round key in forward (encryption) order.
    /// </param>
    /// <param name="invertedRoundKeys">
    /// The destination buffer that receives the inverted round keys in reverse order with the inverse
    /// MixColumns transformation applied to the intermediate rounds. Must be at least 240 bytes in length
    /// and aligned to 16-byte boundaries.
    /// </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InvertKeySchedule(this ReadOnlySpan<byte> roundKeys, Span<byte> invertedRoundKeys) 
        => AesEcb256Ni.InvertKeySchedule(roundKeys, invertedRoundKeys);
}