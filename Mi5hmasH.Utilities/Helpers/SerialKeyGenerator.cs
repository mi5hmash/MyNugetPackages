using static System.Security.Cryptography.RandomNumberGenerator;

namespace Mi5hmasH.Utilities.Helpers;

public static class SerialKeyGenerator
{
    private static string RandomSerialKey(string charsString, char separator = '-', int[]? blocks = null)
    {
        // Set default blocks if null
        blocks ??= [5, 5, 5, 5];

        // create answerParts collection then join the parts with the separator and return
        var serialParts = blocks.Select(block => GetString(charsString, block)).ToList();
        return string.Join(separator, serialParts);
    }

    private static string RandomSerialKeyFromPattern(string pattern, char patternChar, string charsString)
    {
        // split pattern to array
        var charArray = pattern.ToCharArray();
        for (var i = 0; i < pattern.Length; i++)
            if (charArray[i] == patternChar) charArray[i] = charsString.GetRandomChar();
        // convert charArray to string
        return new string(charArray);
    }

    /// <summary>
    /// Generates a serial-key made of blocks of strings of given length, separated by separator.
    /// This variant allows user to specify which characters can be used.
    /// </summary>
    /// <param name="charsString">A string made of all characters that can be used.</param>
    /// <param name="blocks">An array of int with each block length.</param>
    /// <param name="separator">A character to put between blocks.</param>
    /// <returns></returns>
    public static string GenerateSerialKey(string charsString, int[]? blocks = null, char separator = '-') 
        => RandomSerialKey(charsString, separator, blocks);
    
    /// <summary>
    /// Generates a serial-key based on provided pattern.
    /// This variant allows user to specify which characters can be used.
    /// </summary>
    /// <param name="charsString">A string made of all characters that can be used.</param>
    /// <param name="pattern">The pattern of serial key to be generated (f.ex.: "RAND-xxxx-xxxx" where "x" stands for a random character).</param>
    /// <param name="patternChar">A character which stands for a random character (default is "x").</param>
    /// <returns></returns>
    public static string GenerateSerialKeyFromPattern(string charsString, string pattern = "RAND-xxxx-xxxx", char patternChar = 'x')
        => RandomSerialKeyFromPattern(pattern, patternChar, charsString);
}