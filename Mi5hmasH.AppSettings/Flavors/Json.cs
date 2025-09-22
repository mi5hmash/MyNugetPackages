using System.Text.Json;

namespace Mi5hmasH.AppSettings.Flavors;

public class Json : IAppSettingsFlavor
{
    public string FileExtension => ".json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public string Serialize<T>(AppSettingsModel<T> model) where T : new() => JsonSerializer.Serialize(model, Options);

    public AppSettingsModel<T>? Deserialize<T>(string data) where T : new() => JsonSerializer.Deserialize<AppSettingsModel<T>>(data, Options);
}