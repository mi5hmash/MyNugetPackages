using System.Globalization;

namespace Mi5hmasH.WpfHelper.Services;

/// <summary>
/// Defines a service for setting the culture of the application.
/// </summary>
public interface ILocalizationService
{
    void SetCulture(string culture);
}

/// <summary>
/// Localization service for setting the culture of the application.
/// </summary>
public class LocalizationService : ILocalizationService
{
    /// <summary>
    /// Sets the culture of the application to the specified culture.
    /// </summary>
    /// <param name="culture">A string representation of a new culture to set.</param>
    public void SetCulture(string culture)
    {
        var ci = new CultureInfo(culture);

        CultureInfo.DefaultThreadCurrentCulture = ci;
        CultureInfo.DefaultThreadCurrentUICulture = ci;

        CultureInfo.CurrentCulture = ci;
        CultureInfo.CurrentUICulture = ci;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalizationService"/> class with the specified culture.
    /// </summary>
    /// <param name="culture">A string representation of a new culture to set.</param>
    public LocalizationService(string culture)
    {
        SetCulture(culture);
    }
}
