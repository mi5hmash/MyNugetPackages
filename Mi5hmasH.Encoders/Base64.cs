using System.Text;

namespace Mi5hmasH.Encoders;

/// <summary>
/// Provides extension methods for encoding and decoding data using Base64 and for converting between strings and byte arrays with specified encodings.
/// </summary>
public static class Base64
{
    /// <summary>
    /// Converts a byte array into a Base64 string. 
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string B64Encode(this byte[] bytes) => Convert.ToBase64String(bytes);

    /// <summary>
    /// Converts a string into a Base64 string.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string B64Encode(this string str, Encoding encoding) => str.ToBytes(encoding).B64Encode();
    
    /// <summary>
    /// Converts a Base64 string into a byte array.
    /// </summary>
    /// <param name="base64Str"></param>
    /// <returns></returns>
    public static byte[] B64Decode(this string base64Str) => Convert.FromBase64String(base64Str);

    /// <summary>
    /// Converts a Base64 string into a string using the specified encoding.
    /// </summary>
    /// <param name="base64Str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string B64Decode(this string base64Str, Encoding encoding) => base64Str.B64Decode().ToString(encoding);

    /// <summary>
    /// Converts a string into a byte array using the specified encoding.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static byte[] ToBytes(this string str, Encoding encoding) => encoding.GetBytes(str);

    /// <summary>
    /// Converts a byte array into a string using the specified encoding.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    public static string ToString(this byte[] bytes, Encoding encoding) => encoding.GetString(bytes);
}