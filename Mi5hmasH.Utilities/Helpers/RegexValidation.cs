using System.Text.RegularExpressions;

namespace Mi5hmasH.Utilities.Helpers;

public static partial class RegexValidation
{
    #region PATTERNS

    /// <summary>
    /// Email Regex Pattern (Template) - max length 253 | max 8 domain levels (.) | country domain length from 2 to 6.
    /// </summary>
    public const string EmailPattern =
        @"^(?=[A-z0-9][A-z0-9@._%+-]{5,253})[A-z0-9._%+-]{1,70}@(?:(?=[A-z0-9-]{1,70}\.)[A-z0-9]+(?:-[A-z0-9]+)*\.){1,8}[A-z]{2,6}$";

    /// <summary>
    /// Generates a regular expression pattern for validating email addresses belonging to a specified domain.
    /// </summary>
    /// <param name="domain">The domain name to match in the email address. Must be a valid domain string; periods will be escaped in the pattern.</param>
    /// <returns>A string containing a regular expression pattern that matches email addresses for the specified domain.</returns>
    public static string EmailCustomPattern(string domain) 
        => @$"^(?=[A-z0-9][A-z0-9@._%+-]{{5,253}})[A-z0-9._%+-]{{1,150}}@((external\.)?{domain.Replace(".","\\.")})$";

    /// <summary>
    /// Password Regex Pattern - at least one uppercase letter.
    /// </summary>
    public const string UpperCasedLetterPattern =
        "[A-ZĄĆĘŁŃÓŚŹŻĈĜĤĴŜŬÄÖÜ]";

    /// <summary>
    /// Password Regex Pattern - at least one lowercase letter.
    /// </summary>
    public const string LowerCasedLetterPattern =
        "[a-ząćęłńóśźżĉĝĥĵŝŭäöü]";

    /// <summary>
    /// Password Regex Pattern - at least one digit.
    /// </summary>
    public const string DigitPattern =
        "[0-9]";

    /// <summary>
    /// Password Regex Pattern - at least one special character.
    /// </summary>
    public const string SpecialCharacterPattern =
        @"[ ~!""_#$%&'()*+,\-./\:;<=>?@[\]{}^\\|`]";

    /// <summary>
    /// Decimal Regex Pattern - max 18 digits before decimal point and 2 after.
    /// </summary>
    public const string DecimalPattern =
        @"^\d{1,18}[,.]{1}\d{2}$";

    #endregion


    #region GENERATED REGEXES
    
    [GeneratedRegex(DecimalPattern)]
    private static partial Regex DecimalRegex();

    [GeneratedRegex(DigitPattern)]
    private static partial Regex DigitRegex();

    [GeneratedRegex(LowerCasedLetterPattern)]
    private static partial Regex LowerCasedLetterRegex();

    [GeneratedRegex(UpperCasedLetterPattern)]
    private static partial Regex UpperCasedLetterRegex();

    [GeneratedRegex(SpecialCharacterPattern)]
    private static partial Regex SpecialCharacterRegex();

    #endregion


    #region VALIDATION METHODS

    /// <param name="input">A string to examine.</param>
    extension(string input)
    {
        /// <summary>Validates if input string has at least one uppercase letter.</summary>
        public bool HasAtLeastOneUppercaseLetter()
            => UpperCasedLetterRegex().IsMatch(input);

        /// <summary>Validates if input string has at least one lowercase letter.</summary>
        public bool HasAtLeastOneLowercaseLetter()
            => LowerCasedLetterRegex().IsMatch(input);

        /// <summary>Validates if input string has at least one digit.</summary>
        public bool HasAtLeastOneDigit()
            => DigitRegex().IsMatch(input);

        /// <summary>Validates if input string has at least one special character.</summary>
        public bool HasAtLeastOneSpecialCharacter()
            => SpecialCharacterRegex().IsMatch(input);

        /// <summary>Validates if the length of input string is proper.</summary>
        /// <param name="minLength">A minimum length the string must be.</param>
        /// <param name="maxLength">A maximum length the string must be.</param>
        public bool IsProperLength(uint minLength = 8, uint maxLength = 55)
            => (input.Length >= minLength) & (input.Length <= maxLength);

        /// <summary>Validates if the input string is decimal.</summary>
        public bool IsDecimal()
            => DecimalRegex().IsMatch(input);

        /// <summary>Validates if the input string is an Email Address.</summary>
        /// <param name="customEmailPattern">A custom pattern for email validation.</param>
        public bool IsEmail(string? customEmailPattern = null)
            => new Regex(customEmailPattern ?? EmailPattern).IsMatch(input);
    }

    #endregion
}