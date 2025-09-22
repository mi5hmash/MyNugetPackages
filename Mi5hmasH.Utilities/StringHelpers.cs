namespace Mi5hmasH.Utilities;

public static class StringHelpers
{
    /// <summary>
    /// Removes all digits from the end of an <paramref name="inputString"/>.
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns>String without trailing numbers.</returns>
    public static string RemoveSuffixNumbers(this string inputString)
    {
        for (var i = 0; i < inputString.Length; i++)
        {
            if (!char.IsDigit(inputString[^(i + 1)])) 
                return i == 0 ? inputString : inputString[..^i];
        }
        return string.Empty;
    }

    /// <summary>
    /// Gets specific number of characters <paramref name="characterCount"/> from the left side of <paramref name="inputString"/>
    /// </summary>
    /// <param name="inputString"></param>
    /// <param name="characterCount"></param>
    /// <returns>Substring of the given length from the left.</returns>
    public static string Left(this string inputString, int characterCount)
    {
        if (string.IsNullOrEmpty(inputString) || characterCount <= 0) return string.Empty;
        return inputString[..Math.Min(characterCount, inputString.Length)];
    }

    /// <summary>
    /// Gets specific number of characters <paramref name="characterCount"/> from the right side of <paramref name="inputString"/>
    /// </summary>
    /// <param name="inputString"></param>
    /// <param name="characterCount"></param>
    /// <returns>Substring of the given length from the right.</returns>
    public static string Right(this string inputString, int characterCount)
    {
        if (string.IsNullOrEmpty(inputString) || characterCount <= 0) return string.Empty;
        return inputString[Math.Max(0, inputString.Length - characterCount)..];
    }
}