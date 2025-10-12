namespace Mi5hmasH.Utilities.Helpers;

public enum VersionComparisonResult
{
    Older = -1,
    Equal = 0,
    Newer = 1
}

public static class VersionExtensions
{
    private static Version ParseOrDefault(string versionString)
        => Version.TryParse(versionString, out var v) ? v : new Version(0, 0, 0, 0);

    extension(string versionString)
    {
        public bool IsAtLeast(string minVersionString)
            => ParseOrDefault(versionString).CompareTo(ParseOrDefault(minVersionString)) >= 0;

        public bool IsNewerThan(string otherVersionString)
            => ParseOrDefault(versionString).CompareTo(ParseOrDefault(otherVersionString)) > 0;

        public bool IsOlderThan(string otherVersionString)
            => ParseOrDefault(versionString).CompareTo(ParseOrDefault(otherVersionString)) < 0;

        public bool IsBetween(string minVersionString, string maxVersionString, bool inclusive = true)
        {
            var version = ParseOrDefault(versionString);
            var min = ParseOrDefault(minVersionString);
            var max = ParseOrDefault(maxVersionString);

            return inclusive
                ? version.CompareTo(min) >= 0 && version.CompareTo(max) <= 0
                : version.CompareTo(min) > 0 && version.CompareTo(max) < 0;
        }

        public VersionComparisonResult CompareToVersion(string otherVersionString)
        {
            var result = ParseOrDefault(versionString).CompareTo(ParseOrDefault(otherVersionString));
            return (VersionComparisonResult)result;
        }
    }
}