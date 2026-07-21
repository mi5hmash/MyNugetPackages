using System.Runtime.InteropServices;

namespace Mi5hmasH.Utilities.Helpers;

public static class OsVersion
{
    /// <summary>
    /// Determines whether the current operating system is Windows 10 with a build number greater than or equal to 17763
    /// (Windows 10 version 1809) and less than 22000 (the first Windows 11 build).
    /// </summary>
    /// <returns><see langword="true"/> if the operating system is Windows 10 with a build number between 17763 (inclusive) and 22000 (exclusive); otherwise, <see langword="false"/>.</returns>
    public static bool IsWindows10GreaterThan1809()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;
        var version = Environment.OSVersion.Version;
        return version is { Major: >= 10, Build: >= 17763 and < 22000 };
    }
}