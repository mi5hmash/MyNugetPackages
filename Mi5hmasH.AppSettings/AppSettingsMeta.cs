using System.Reflection;

namespace Mi5hmasH.AppSettings;

/// <summary>
/// The Meta section of the <see cref="AppSettingsModel{T}"/> file.
/// </summary>
public class AppSettingsMeta
{
    public string Title { get; set; } = GetTitle();
    public string Version { get; set; } = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0.0";

    /// <summary>
    /// Retrieves the title of the entry assembly as specified by the <see cref="AssemblyTitleAttribute"/>.
    /// </summary>
    /// <returns>A string containing the title of the entry assembly, or an empty string if the title attribute is not defined.</returns>
    private static string GetTitle()
    {
        var attributes = Assembly.GetEntryAssembly()?.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes is not { Length: > 0 }) return string.Empty;
        var titleAttribute = attributes[0] as AssemblyTitleAttribute;
        return titleAttribute?.Title ?? "MyApp";
    }
}