using System.Runtime.InteropServices;

namespace Mi5hmasH.Utilities.Helpers;

public static class OsPlatform
{
    /// <summary>
    /// Detects the current operating system platform and returns its name as a string.
    /// </summary>
    /// <returns>A string representing the current operating system platform.</returns>
    public static string GetOsPlatform()
    {
        if (OperatingSystem.IsWindows()) return "Windows";
        if (OperatingSystem.IsLinux()) return "Linux";
        if (OperatingSystem.IsFreeBSD()) return "FreeBSD";
        return OperatingSystem.IsMacOS() ? "MacOS" : "Unknown";
    }

    /// <summary>
    /// Retrieves a descriptive string that identifies the current operating system.
    /// </summary>
    /// <returns>A string containing the operating system description. The string is trimmed of leading and trailing whitespace.</returns>
    public static string GetOsDescription()
        => RuntimeInformation.OSDescription.Trim();
}