using System.Text.RegularExpressions;

namespace Mi5hmasH.Utilities;

public static partial class RegexValidation
{
    #region PATTERNS

    /// <summary>
    /// Email Regex Pattern (Template) - max length 253 | max 8 domain levels (.) | country domain length from 2 to 6.
    /// </summary>
    public const string EmailPattern =
        @"^(?=[A-z0-9][A-z0-9@._%+-]{5,253})[A-z0-9._%+-]{1,70}@(?:(?=[A-z0-9-]{1,70}\.)[A-z0-9]+(?:-[A-z0-9]+)*\.){1,8}[A-z]{2,6}$";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public static string EmailCustomPattern(string domain) => @$"^(?=[A-z0-9][A-z0-9@._%+-]{{5,253}})[A-z0-9._%+-]{{1,150}}@((external\.)?{domain.Replace(".","\\.")})$";

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

    /// <summary>Validates if input string has at least one uppercase letter.</summary>
    /// <param name="input">A string to examine.</param>
    public static bool HasAtLeastOneUppercaseLetter(this string input)
        => UpperCasedLetterRegex().IsMatch(input);

    /// <summary>Validates if input string has at least one lowercase letter.</summary>
    /// <param name="input">A string to examine.</param>
    public static bool HasAtLeastOneLowercaseLetter(this string input)
        => LowerCasedLetterRegex().IsMatch(input);

    /// <summary>Validates if input string has at least one digit.</summary>
    /// <param name="input">A string to examine.</param>
    public static bool HasAtLeastOneDigit(this string input)
        => DigitRegex().IsMatch(input);

    /// <summary>Validates if input string has at least one special character.</summary>
    /// <param name="input">A string to examine.</param>
    public static bool HasAtLeastOneSpecialCharacter(this string input)
        => SpecialCharacterRegex().IsMatch(input);

    /// <summary>Validates if the length of input string is proper.</summary>
    /// <param name="input">A string to examine.</param>
    /// <param name="minLength">A minimum length the string must be.</param>
    /// <param name="maxLength">A maximum length the string must be.</param>
    public static bool IsProperLength(this string input, uint minLength = 8, uint maxLength = 55)
        => (input.Length >= minLength) & (input.Length <= maxLength);

    /// <summary>Validates if the input string is decimal.</summary>
    /// <param name="input">A string to examine.</param>
    public static bool IsDecimal(this string input)
        => DecimalRegex().IsMatch(input);

    /// <summary>Validates if the input string is an Email Address.</summary>
    /// <param name="input">A string to examine.</param>
    /// <param name="customEmailPattern">A custom pattern for email validation.</param>
    public static bool IsEmail(this string input, string? customEmailPattern = null)
        => new Regex(customEmailPattern ?? EmailPattern).IsMatch(input);

    #endregion
}