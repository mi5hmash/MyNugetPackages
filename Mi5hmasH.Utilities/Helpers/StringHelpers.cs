namespace Mi5hmasH.Utilities.Helpers;

public static class StringHelpers
{
    /// <param name="inputString">A string to process.</param>
    extension(string inputString)
    {
        /// <summary>
        /// Removes all digits from the end of an <paramref name="inputString"/>.
        /// </summary>
        /// <returns>String without trailing numbers.</returns>
        public string RemoveSuffixNumbers()
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
        /// <param name="characterCount"></param>
        /// <returns>Substring of the given length from the left.</returns>
        public string Left(int characterCount)
        {
            if (string.IsNullOrEmpty(inputString) || characterCount <= 0) return string.Empty;
            return inputString[..Math.Min(characterCount, inputString.Length)];
        }

        /// <summary>
        /// Gets specific number of characters <paramref name="characterCount"/> from the right side of <paramref name="inputString"/>
        /// </summary>
        /// <param name="characterCount"></param>
        /// <returns>Substring of the given length from the right.</returns>
        public string Right(int characterCount)
        {
            if (string.IsNullOrEmpty(inputString) || characterCount <= 0) return string.Empty;
            return inputString[Math.Max(0, inputString.Length - characterCount)..];
        }
    }
}