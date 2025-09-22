using Microsoft.Win32;
using System.Runtime.InteropServices;
using static System.Environment;
using static System.Runtime.InteropServices.RuntimeInformation;

namespace Mi5hmasH.GameLaunchers.Steam;

public static class SteamHelpers
{
    public static string GetSteamPath()
    {
        if (IsOSPlatform(OSPlatform.Windows))
        {
            using var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Valve\Steam");
            return (string?)reg?.GetValue("InstallPath") ?? string.Empty;
        }
        if (IsOSPlatform(OSPlatform.OSX))
        {
            var macPath = Path.Combine(
                GetFolderPath(SpecialFolder.Personal),
                "Library/Application Support/Steam"
            );
            return Directory.Exists(macPath) ? macPath : string.Empty;
        }

        if (!IsOSPlatform(OSPlatform.Linux) && !IsOSPlatform(OSPlatform.FreeBSD)) return string.Empty;
        string[] possiblePaths =
        [
            Path.Combine(GetFolderPath(SpecialFolder.Personal), ".steam/steam"),
            Path.Combine(GetFolderPath(SpecialFolder.Personal), ".local/share/Steam")
        ];

        foreach (var path in possiblePaths)
        {
            if (Directory.Exists(path))
                return path;
        }

        return string.Empty;
    }
}