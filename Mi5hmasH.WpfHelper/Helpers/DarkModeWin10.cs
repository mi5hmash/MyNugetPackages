using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Mi5hmasH.Utilities.Helpers;
using Microsoft.Win32;

namespace Mi5hmasH.WpfHelper.Helpers;

public static partial class DarkModeWin10
{
    private const int DwmwaUseImmersiveDarkMode = 20;

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

    /// <summary>
    /// Enables or disables immersive dark mode for the specified window based on the current system dark mode setting.
    /// </summary>
    /// <param name="window">The WPF <see cref="Window"/> instance for which immersive dark mode should be applied.</param>
    public static void FixImmersiveDarkMode(this Window window)
    {
        if (!OsVersion.IsWindows10GreaterThan1809()) return;
        var useDark = IsDarkModeEnabled();
        var useDarkInt = useDark ? 1 : 0;
        // Fix Title Bar
        var hwnd = new WindowInteropHelper(window).Handle;
        _ = DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkMode, ref useDarkInt, sizeof(int));
        // Fix Window Background
        window.Background = useDark ? System.Windows.Media.Brushes.Black : System.Windows.Media.Brushes.White;
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