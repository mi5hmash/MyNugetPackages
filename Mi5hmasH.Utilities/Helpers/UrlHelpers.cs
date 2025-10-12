using System.Diagnostics;

namespace Mi5hmasH.Utilities.Helpers;

public static class UrlHelpers
{
    /// <summary>
    /// Opens the given URL in the default web browser.
    /// </summary>
    /// <param name="url"></param>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public static void OpenUrl(this string url)
    {
        if (OperatingSystem.IsWindows())
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        else if (OperatingSystem.IsLinux() || OperatingSystem.IsFreeBSD())
            Process.Start("xdg-open", url);
        else if (OperatingSystem.IsMacOS())
            Process.Start("open", url);
        else
            throw new PlatformNotSupportedException("Unsupported OS platform.");
    }
}