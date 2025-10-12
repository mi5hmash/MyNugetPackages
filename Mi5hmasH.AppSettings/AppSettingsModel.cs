namespace Mi5hmasH.AppSettings;

public class AppSettingsModel<T> where T : new()
{
    /// <summary>
    /// Gets or sets metadata information related to the application settings.
    /// </summary>
    public AppSettingsMeta Meta { get; set; } = new();

    /// <summary>
    /// Gets or sets the application-specific settings used to configure behavior.
    /// </summary>
    public T AppSettings { get; set; } = new();
}