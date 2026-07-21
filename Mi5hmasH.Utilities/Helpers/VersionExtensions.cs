namespace Mi5hmasH.Utilities.Helpers;

/// <summary>
/// Enumeration representing the result of a version comparison.
/// </summary>
public enum VersionComparisonResult
{
    Older = -1,
    Equal = 0,
    Newer = 1
}

/// <summary>
/// Provides extension methods for comparing version strings.
/// </summary>
public static class VersionExtensions
{
    /// <summary>
    /// Provides extension methods for comparing version strings.
    /// </summary>
    /// <param name="versionString">The version string to compare.</param>
    extension(string versionString)
    {
        /// <summary>
        /// Determines if the current version string is at least the specified minimum version string.
        /// </summary>
        /// <param name="minVersionString">The minimum version string to compare with.</param>
        /// <returns><see langword="true"/> if the current version is at least the specified minimum version; otherwise, <see langword="false"/>.</returns>
        public bool IsAtLeast(string minVersionString)
            => versionString.ParseOrDefaultVersion().CompareTo(minVersionString.ParseOrDefaultVersion()) >= 0;

        /// <summary>
        /// Determines if the current version string is newer than the specified version string.
        /// </summary>
        /// <param name="otherVersionString">The version string to compare with.</param>
        /// <returns><see langword="true"/> if the current version is newer than the specified version; otherwise, <see langword="false"/>.</returns>
        public bool IsNewerThan(string otherVersionString)
            => versionString.ParseOrDefaultVersion().CompareTo(otherVersionString.ParseOrDefaultVersion()) > 0;

        /// <summary>
        /// Determines if the current version string is older than the specified version string.
        /// </summary>
        /// <param name="otherVersionString">The version string to compare with.</param>
        /// <returns><see langword="true"/> if the current version is older than the specified version; otherwise, <see langword="false"/>.</returns>
        public bool IsOlderThan(string otherVersionString)
            => versionString.ParseOrDefaultVersion().CompareTo(otherVersionString.ParseOrDefaultVersion()) < 0;

        /// <summary>
        /// Determines if the current version string is between the specified minimum and maximum version strings.
        /// </summary>
        /// <param name="minVersionString">The minimum version string to compare with.</param>
        /// <param name="maxVersionString">The maximum version string to compare with.</param>
        /// <param name="inclusive">Whether the comparison is inclusive of the minimum and maximum versions.</param>
        /// <returns><see langword="true"/> if the current version is between the specified versions; otherwise, <see langword="false"/>.</returns>
        public bool IsBetween(string minVersionString, string maxVersionString, bool inclusive = true)
        {
            var thisVersion = versionString.ParseOrDefaultVersion();
            var minVersion = minVersionString.ParseOrDefaultVersion();
            var maxVersion = maxVersionString.ParseOrDefaultVersion();

            return inclusive
                ? thisVersion.CompareTo(minVersion) >= 0 
                  && thisVersion.CompareTo(maxVersion) <= 0
                : thisVersion.CompareTo(minVersion) > 0 
                  && thisVersion.CompareTo(maxVersion) < 0;
        }

        /// <summary>
        /// Compares the current version string with another version string.
        /// </summary>
        /// <param name="otherVersionString">The version string to compare with.</param>
        /// <returns>A <see cref="VersionComparisonResult"/> indicating whether the current version is older, equal, or newer than the specified version.</returns>
        public VersionComparisonResult CompareToVersion(string otherVersionString)
        {
            var thisVersion = versionString.ParseOrDefaultVersion();
            var otherVersion = otherVersionString.ParseOrDefaultVersion();
            var result = thisVersion.CompareTo(otherVersion);
            return (VersionComparisonResult)result;
        }

        /// <summary>
        /// Parses a version string into a Version object. If parsing fails, returns a default Version.
        /// </summary>
        /// <returns>A <see cref="Version"/> object representing the parsed version string, or a default <see cref="Version"/> if parsing fails.</returns>
        private Version ParseOrDefaultVersion()
            => Version.TryParse(versionString, out var v) ? v : new Version(0, 0, 0, 0);
    }
}