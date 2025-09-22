namespace Mi5hmasH.AppSettings.Flavors;

public interface IAppSettingsFlavor
{
    string FileExtension { get; }
    string Serialize<T>(AppSettingsModel<T> model) where T : new();
    AppSettingsModel<T>? Deserialize<T>(string filePath) where T : new();
}