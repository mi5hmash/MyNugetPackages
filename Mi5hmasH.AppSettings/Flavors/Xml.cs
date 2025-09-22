using System.Xml.Serialization;

namespace Mi5hmasH.AppSettings.Flavors;

public class Xml : IAppSettingsFlavor
{
    public string FileExtension => ".xml";

    public string Serialize<T>(AppSettingsModel<T> model) where T : new()
    {
        var serializer = new XmlSerializer(typeof(AppSettingsModel<T>));
        using var writer = new StringWriter();
        serializer.Serialize(writer, model);
        return writer.ToString();
    }

    public AppSettingsModel<T>? Deserialize<T>(string data) where T : new()
    {
        var serializer = new XmlSerializer(typeof(AppSettingsModel<T>));
        using var reader = new StringReader(data);
        return serializer.Deserialize(reader) as AppSettingsModel<T>;
    }
}