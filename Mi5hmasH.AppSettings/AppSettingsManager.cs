using Mi5hmasH.AppSettings.Encryption;
using Mi5hmasH.AppSettings.Flavors;

namespace Mi5hmasH.AppSettings;

/// <summary>
/// Manages application settings by providing functionality to save, load, and optionally encrypt settings data.
/// </summary>
/// <typeparam name="T1">The type representing the application settings configuration.</typeparam>
/// <typeparam name="T2">The type representing the settings flavor, which must implement <see cref="IAppSettingsFlavor"/> and have a parameterless constructor.</typeparam>
/// <param name="settings">The settings configuration for the current instance.</param>
/// <param name="directory">The directory path where setting file is stored.</param>
public class AppSettingsManager<T1, T2>(T1 settings, string directory) where T1 : new() where T2 : IAppSettingsFlavor, new()
{
    /// <summary>
    /// The default file name used for application settings.
    /// </summary>
    private const string FileName = "appsettings";

    /// <summary>
    /// Represents the default flavor instance of type <typeparamref name="T2"/>.
    /// </summary>
    private readonly T2 _flavor = new();

    /// <summary>
    /// Gets the directory path where setting file is stored.
    /// </summary>
    public string FileDirectory { get; } = directory;
    
    /// <summary>
    /// Gets or sets the settings configuration for the current instance.
    /// </summary>
    public T1 Settings { get; set; } = settings;
    

    #region ENCRYPTION

    /// <summary>
    /// Gets or sets the AES encryption utility used for cryptographic operations.
    /// </summary>
    private AesCrypto? Encryptor { get; set; }

    /// <summary>
    /// Sets the encryptor to be used for encryption operations.
    /// </summary>
    /// <param name="encryptor">An <see cref="AesCrypto"/> instance that provides the encryption functionality.</param>
    public void SetEncryptor(AesCrypto encryptor)
        => Encryptor = encryptor;

    /// <summary>
    /// Represents the file extension used for encrypted files.
    /// </summary>
    private const string EncryptedFileExtension = ".bin";

    /// <summary>
    /// Constructs the full file path for the encrypted version of the settings file.
    /// </summary>
    /// <returns></returns>
    private string GetFilePathEncrypted() 
        => Path.Combine(FileDirectory, $"{FileName}{EncryptedFileExtension}");

    #endregion


    /// <summary>
    /// Constructs the full file path for the settings file.
    /// </summary>
    /// <returns></returns>
    private string GetFilePath() 
        => Path.Combine(FileDirectory, $"{FileName}{_flavor.FileExtension}");

    /// <summary>
    /// Saves the current application settings to a file.
    /// </summary>
    public void Save()
    {
        // prepare the data
        var newSettings = new AppSettingsModel<T1?>(Settings);
        var data = _flavor.Serialize(newSettings);
        // save the data
        if (Encryptor == null) File.WriteAllText(GetFilePath(), data);
        else
        {
            var encryptedString = Encryptor.Encrypt(data);
            File.WriteAllText(GetFilePathEncrypted(), encryptedString);
        }
    }

    /// <summary>
    /// Loads the application settings from a file, optionally decrypting the data if an encryptor is provided.
    /// </summary>
    /// <exception cref="FileNotFoundException">Thrown if the settings file cannot be found.</exception>
    /// <exception cref="Exception">Thrown if the settings file cannot be deserialized.</exception>
    public void Load()
    {
        // check if the file exists
        if (!File.Exists(GetFilePath()) && !File.Exists(GetFilePathEncrypted()))
            throw new FileNotFoundException("Settings file not found.", GetFilePath());
        // try to load the data
        var data = Encryptor == null 
            ? File.ReadAllText(GetFilePath()) 
            : Encryptor.Decrypt(File.ReadAllText(GetFilePathEncrypted()));
        var newSettings = _flavor.Deserialize<T1>(data) ?? throw new Exception("Failed to deserialize settings.");
        if (newSettings.AppSettings != null) Settings = newSettings.AppSettings;
    }
}