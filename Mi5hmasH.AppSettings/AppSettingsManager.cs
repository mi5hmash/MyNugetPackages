using Mi5hmasH.AppSettings.Flavors;
using Mi5hmasH.Utilities;

namespace Mi5hmasH.AppSettings;

/// <summary>
/// Manages application settings by providing functionality to save, load, and optionally encrypt settings data.
/// </summary>
/// <typeparam name="T1">The type representing the application settings configuration.</typeparam>
/// <typeparam name="T2">The type representing the settings flavor, which must implement <see cref="IAppSettingsFlavor"/> and have a parameterless constructor.</typeparam>
public class AppSettingsManager<T1, T2> where T1 : new() where T2 : IAppSettingsFlavor, new()
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
    public string FileDirectory { get; }

    /// <summary>
    /// The internal model that holds both metadata and application settings.
    /// </summary>
    private AppSettingsModel<T1> _appSettingsModel = new();

    /// <summary>
    /// Gets or sets the settings configuration for the current instance.
    /// </summary>
    public T1 Settings
    {
        get => _appSettingsModel.AppSettings;
        set => _appSettingsModel.AppSettings = value;
    }

    /// <summary>
    /// Gets the metadata information associated with the application settings.
    /// </summary>
    public AppSettingsMeta Meta
    {
        get => _appSettingsModel.Meta;
        private init => _appSettingsModel.Meta = value;
    }

    /// <summary>
    /// Gets the local metadata settings for the application.
    /// </summary>
    private AppSettingsMeta LocalMeta { get; }

    #region ENCRYPTION

    /// <summary>
    /// Gets or sets the AES encryption utility used for cryptographic operations.
    /// </summary>
    private AesCrypto? Encryptor { get; set; }

    /// <summary>
    /// Determines whether encryption is currently enabled for this instance.
    /// </summary>
    /// <returns><see langword="true"/> if encryption is enabled; otherwise, <see langword="false"/>.</returns>
    public bool EncryptionEnabled() => Encryptor != null;

    /// <summary>
    /// Sets the encryptor.
    /// </summary>
    /// <param name="key">The encryption key to use for initializing the encryptor.</param>
    public void SetEncryptor(string key)
        => Encryptor = new AesCrypto(key);

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
    /// Initializes a new instance of the AppSettingsManager class with optional metadata and a settings directory.
    /// </summary>
    /// <param name="localMeta">The local metadata to use for application settings. If null, a new AppSettingsMeta instance is created.</param>
    /// <param name="directory">The path to the directory where settings files are stored. If null, the application's base directory is used.</param>
    public AppSettingsManager(AppSettingsMeta? localMeta = null, string? directory = null)
    {
        LocalMeta = localMeta ?? new AppSettingsMeta();
        Meta = new AppSettingsMeta();
        Settings = new T1();
        FileDirectory = directory ?? AppDomain.CurrentDomain.BaseDirectory;
        Directory.CreateDirectory(FileDirectory);
    }
    
    /// <summary>
    /// Loads application settings from the settings file, optionally decrypting the file if an encryptor is configured.
    /// </summary>
    /// <exception cref="FileNotFoundException">Thrown if the settings file or encrypted settings file does not exist at the expected location.</exception>
    /// <exception cref="Exception">Thrown if the settings file cannot be deserialized, or if the file's title does not match the application's expected title.</exception>
    public void Load()
    {
        // try to load the data
        string data;
        const string errorMessage = "Settings file not found.";
        if (Encryptor == null)
        {
            if (!File.Exists(GetFilePath()))
                throw new FileNotFoundException(errorMessage, GetFilePath());
            data = File.ReadAllText(GetFilePath());
        }
        else
        {
            if (!File.Exists(GetFilePathEncrypted()))
                throw new FileNotFoundException(errorMessage, GetFilePathEncrypted());
            data = Encryptor.Decrypt(File.ReadAllText(GetFilePathEncrypted()));
        }
        var newSettings = _flavor.Deserialize<T1>(data) ?? throw new Exception("Failed to deserialize settings.");
        if (LocalMeta.Title != newSettings.Meta.Title)
            throw new Exception($"Settings file title '{newSettings.Meta.Title}' does not match application title '{LocalMeta.Title}'.");
        _appSettingsModel = newSettings;
    }

    /// <summary>
    /// Loads the specified settings into the current instance, updating metadata and configuration accordingly.
    /// </summary>
    /// <param name="settings">The settings object to apply.</param>
    public void Load(T1 settings)
    {
        Meta.Title = LocalMeta.Title;
        Meta.Version = LocalMeta.Title;
        Settings = settings;
    }

    /// <summary>
    /// Saves the current application settings to a file.
    /// </summary>
    public void Save()
    {
        // prepare the data
        var data = _flavor.Serialize(_appSettingsModel);
        // save the data
        if (Encryptor == null) File.WriteAllText(GetFilePath(), data);
        else
        {
            var encryptedString = Encryptor.Encrypt(data);
            File.WriteAllText(GetFilePathEncrypted(), encryptedString);
        }
    }
}