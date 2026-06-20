using System.Text;

namespace Mi5hmasH.Encoders;

/// <summary>
/// Provides extension methods for encoding and decoding data using Base64 and for converting between strings and byte arrays with specified encodings.
/// </summary>
public static class Base64
{
    /// <summary>
    /// Encodes the specified byte array to a Base64 string.
    /// </summary>
    /// <param name="bytes">The byte array to encode.</param>
    /// <returns>The Base64 encoded string.</returns>
    public static string B64Encode(this byte[] bytes) => Convert.ToBase64String(bytes);

    /// <param name="str">The string to be processed.</param>
    extension(string str)
    {
        /// <summary>
        /// Converts a string into a Base64 string.
        /// </summary>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <returns>The Base64 encoded string.</returns>
        public string B64Encode(Encoding encoding) => str.ToBytes(encoding).B64Encode();

        /// <summary>
        /// Converts a Base64 string into a byte array.
        /// </summary>
        /// <returns>The decoded byte array.</returns>
        public byte[] B64Decode() => Convert.FromBase64String(str);

        /// <summary>
        /// Converts a Base64 string into a string using the specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <returns>The decoded string.</returns>
        public string B64Decode(Encoding encoding) => str.B64Decode().ToString(encoding);

        /// <summary>
        /// Converts a string into a byte array using the specified encoding.
        /// </summary>
        /// <param name="encoding">The encoding to use for the conversion.</param>
        /// <returns>The byte array representation of the string.</returns>
        public byte[] ToBytes(Encoding encoding) => encoding.GetBytes(str);
    }

    /// <summary>
    /// Converts a byte array into a string using the specified encoding.
    /// </summary>
    /// <param name="bytes">The byte array to convert.</param>
    /// <param name="encoding">The encoding to use for the conversion.</param>
    /// <returns>The string representation of the byte array.</returns>
    public static string ToString(this byte[] bytes, Encoding encoding) => encoding.GetString(bytes);
}