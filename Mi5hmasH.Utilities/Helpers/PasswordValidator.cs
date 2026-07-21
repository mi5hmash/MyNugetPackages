using static Mi5hmasH.Utilities.Helpers.RandomPasswordGenerator;

namespace Mi5hmasH.Utilities.Helpers;

public static class PasswordValidator
{
    /// <param name="password">A password to examine.</param>
    extension(string password)
    {
        /// <summary>
        /// Validates password against a predefined standard.
        /// </summary>
        /// <param name="minLength">A minimum length the password must be.</param>
        /// <param name="maxLength">A maximum length the password must be.</param>
        /// <param name="uppercaseChars">Determines if password should contain at least one uppercase character.</param>
        /// <param name="lowercaseChars">Determines if password should contain at least one lowercase character.</param>
        /// <param name="numericChars">Determines if password should contain at least one digit.</param>
        /// <param name="specialChars">Determines if password should contain at least one special character.</param>
        /// <param name="minEntropy">A minimum entropy the password must have.</param>
        /// <returns>A list of validation error messages, empty if the password is compliant.</returns>
        public List<string> ValidatePassword(ushort minLength = 0, ushort maxLength = ushort.MaxValue,
            bool uppercaseChars = false, bool lowercaseChars = false,
            bool numericChars = false, bool specialChars = false, double minEntropy = 0)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
                errors.Add("Password cannot be empty.");
            if (password.Length < minLength)
                errors.Add($"Password must be at least {minLength} characters long.");
            if (password.Length > maxLength)
                errors.Add($"Password cannot exceed {maxLength} characters.");
            if (uppercaseChars && !password.Any(char.IsUpper))
                errors.Add("Password must contain at least one uppercase letter.");
            if (lowercaseChars && !password.Any(char.IsLower))
                errors.Add("Password must contain at least one lowercase letter.");
            if (numericChars && !password.Any(char.IsDigit))
                errors.Add("Password must contain at least one digit.");
            if (specialChars && password.All(char.IsLetterOrDigit))
                errors.Add("Password must contain at least one special character.");
            if (minEntropy > 0 && password.CalculateEntropy() < minEntropy)
                errors.Add($"Password must have a minimum entropy of {minEntropy}.");

            return errors;
        }

        /// <summary>
        /// Calculates entropy of input string.
        /// </summary>
        public double CalculateEntropy()
        {
            // calculate poolSize
            var poolSize = 0;
            if (password.HasAtLeastOneUppercaseLetter()) poolSize += LatinAlphabet.Length;
            if (password.HasAtLeastOneLowercaseLetter()) poolSize += LatinAlphabet.Length;
            if (password.HasAtLeastOneDigit()) poolSize += Digits.Length;
            if (password.HasAtLeastOneSpecialCharacter()) poolSize += SpecialChars.Length;
            // calculate entropy and return the rounded entropy value
            var entropy = Math.Log(poolSize, 2) * password.Length;
            return Math.Round(entropy, 2);
        }
    }
}