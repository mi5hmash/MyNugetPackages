using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

namespace Mi5hmasH.WpfHelper;

public static partial class DarkModeWin10Helper
{
    private const int DwmwaUseImmersiveDarkMode = 20;

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    /// <summary>
    /// Enables or disables immersive dark mode for the specified window based on the current system dark mode setting.
    /// </summary>
    /// <param name="hwnd">A handle to the window for which to apply the immersive dark mode setting.</param>
    public static void FixImmersiveDarkMode(IntPtr hwnd)
    {
        var useDark = IsDarkModeEnabled();
        var useDarkInt = useDark ? 1 : 0;
        // Fix Title Bar
        _ = DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkMode, ref useDarkInt, sizeof(int));
        // Fix Window Background
        var source = HwndSource.FromHwnd(hwnd);
        if (source?.RootVisual is Window window)
            window.Background = useDark ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.White;
    }

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

    /// <summary>
    /// Determines whether dark mode is enabled for Windows applications on the current user account.
    /// </summary>
    /// <returns><see langword="true"/> if dark mode is enabled for Windows applications; otherwise, <see langword="false"/>.</returns>
    public static bool IsDarkModeEnabled()
    {
        const string keyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        using var key = Registry.CurrentUser.OpenSubKey(keyPath);
        var registryValue = key?.GetValue("AppsUseLightTheme");
        return registryValue is 0; // 0 = Dark Mode, 1 = Light Mode
    }
}