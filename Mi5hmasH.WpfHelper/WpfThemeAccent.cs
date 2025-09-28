using System.Text;
using System.Windows;
using Color = System.Windows.Media.Color;
using SystemColors = System.Windows.SystemColors;

namespace Mi5hmasH.WpfHelper;

/// <summary>
/// A model of a Color Accent.
/// </summary>
public record ColorAccentModel(
    Color SystemAccentColor,
    Color SystemAccentColorLight1,
    Color SystemAccentColorLight2,
    Color SystemAccentColorLight3,
    Color SystemAccentColorDark1,
    Color SystemAccentColorDark2,
    Color SystemAccentColorDark3);

public static class WpfThemeAccent
{
    /// <summary>
    /// Adjusts the brightness of a color by scaling its RGB values.
    /// </summary>
    /// <param name="color">The color to adjust.</param>
    /// <param name="factor">The brightness adjustment factor.</param>
    /// <returns>The adjusted color.</returns>
    private static Color ChangeBrightness(this Color color, double factor)
        => Color.FromRgb(
            (byte)Math.Min(255, color.R * factor),
            (byte)Math.Min(255, color.G * factor),
            (byte)Math.Min(255, color.B * factor));
    
    /// <summary>
    /// Dynamically generates an accent model from a base color and applies it.
    /// </summary>
    /// <param name="color">The base color to create the theme.</param>
    public static void SetThemeAccent(Color color)
    {
        var accentModel = new ColorAccentModel(
            color,
            color.ChangeBrightness(1.2),
            color.ChangeBrightness(1.4),
            color.ChangeBrightness(1.6),
            color.ChangeBrightness(0.8),
            color.ChangeBrightness(0.6),
            color.ChangeBrightness(0.4));
        UpdateResourceKeys(accentModel);
    }

    /// <summary>
    /// Applies the specified accent color theme to the application.
    /// </summary>
    /// <param name="colorAccent">The accent color model to use for updating the application's theme.</param>
    public static void SetThemeAccent(ColorAccentModel colorAccent)
        => UpdateResourceKeys(colorAccent);

    /// <summary>
    /// Updates application resources with the provided accent model.
    /// </summary>
    /// <param name="colorAccentModel">The accent model to apply to resources.</param>
    private static void UpdateResourceKeys(ColorAccentModel colorAccentModel)
    {
        Application.Current.Resources[SystemColors.AccentColorKey] = colorAccentModel.SystemAccentColor;
        Application.Current.Resources[SystemColors.AccentColorLight1Key] = colorAccentModel.SystemAccentColorLight1;
        Application.Current.Resources[SystemColors.AccentColorLight2Key] = colorAccentModel.SystemAccentColorLight2;
        Application.Current.Resources[SystemColors.AccentColorLight3Key] = colorAccentModel.SystemAccentColorLight3;
        Application.Current.Resources[SystemColors.AccentColorDark1Key] = colorAccentModel.SystemAccentColorDark1;
        Application.Current.Resources[SystemColors.AccentColorDark2Key] = colorAccentModel.SystemAccentColorDark2;
        Application.Current.Resources[SystemColors.AccentColorDark3Key] = colorAccentModel.SystemAccentColorDark3;
    }

#if DEBUG
    /// <summary>
    /// Formats a color as a string in Color.FromRgb(R, G, B) format.
    /// </summary>
    /// <param name="color">The color to format.</param>
    /// <returns>The formatted color string.</returns>
    private static string GetColorCode(Color color)
        => $"Color.FromRgb({color.R}, {color.G}, {color.B})";

    /// <summary>
    /// Formats a descriptive string for a color with its accent's name and ARGB values represented in HEX.
    /// </summary>
    /// <param name="name">The name of the color's accent.</param>
    /// <param name="color">The HEX representation of ARGB values.</param>
    /// <returns>A formatted string describing the color, including its accent's name and ARGB HEX value (e.g., "Blue: #FF0000FF").</returns>
    private static string GetColorDetails(string name, Color color)
        => $"{name}: {color}";

    /// <summary>
    /// Generates a formatted string containing details of the current system accent colors.
    /// </summary>
    /// <returns>A multi-line string listing the names and values of the system accent colors.
    /// The string includes both light and dark accent color variants.</returns>
    public static string GetAccentColorsText()
    {
        var builder = new StringBuilder();
        builder.AppendLine("===== System Accent Colors =====");
        builder.AppendLine(GetColorDetails("SystemAccentColor", SystemColors.AccentColor));
        builder.AppendLine(GetColorDetails("SystemAccentColorLight1", SystemColors.AccentColorLight1));
        builder.AppendLine(GetColorDetails("SystemAccentColorLight2", SystemColors.AccentColorLight2));
        builder.AppendLine(GetColorDetails("SystemAccentColorLight3", SystemColors.AccentColorLight3));
        builder.AppendLine(GetColorDetails("SystemAccentColorDark1", SystemColors.AccentColorDark1));
        builder.AppendLine(GetColorDetails("SystemAccentColorDark2", SystemColors.AccentColorDark2));
        builder.AppendLine(GetColorDetails("SystemAccentColorDark3", SystemColors.AccentColorDark3));
        builder.AppendLine("================================");

        return builder.ToString();
    }

    /// <summary>
    /// Generates a formatted code string representing accent color definitions for a theme.
    /// </summary>
    /// <returns>A string containing the code representation of accent colors, including their light and dark variants, suitable for use in theme configuration or code generation scenarios.</returns>
    public static string GetAccentColorsCode()
    {
        var builder = new StringBuilder();
        builder.AppendLine("{");
        builder.AppendLine("ThemeColor.NAME_YOUR_COLOR,");
        builder.AppendLine("new ColorAccentModel(");
        builder.AppendLine(GetColorCode(SystemColors.AccentColor) + ",");
        builder.AppendLine(GetColorCode(SystemColors.AccentColorLight1) + ",");
        builder.AppendLine(GetColorCode(SystemColors.AccentColorLight2) + ",");
        builder.AppendLine(GetColorCode(SystemColors.AccentColorLight3) + ",");
        builder.AppendLine(GetColorCode(SystemColors.AccentColorDark1) + ",");
        builder.AppendLine(GetColorCode(SystemColors.AccentColorDark2) + ",");
        builder.AppendLine(GetColorCode(SystemColors.AccentColorDark3) + ");");
        builder.AppendLine("},");

        return builder.ToString();
    }
#endif
}