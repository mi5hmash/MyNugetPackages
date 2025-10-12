using Mi5hmasH.Utilities;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace Mi5hmasH.GameProfile;

public class GameProfileManager<T>(string? directory = null) : INotifyPropertyChanged
    where T : IGameProfile, new()
{
    /// <summary>
    /// Stores the default directory path where game profiles are saved and loaded from.
    /// </summary>
    private readonly string _gpDirectory = directory ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "_profiles");

    /// <summary>
    /// Gets the name of the currently loaded Game Profile.
    /// </summary>
    public string? CurrentlyLoadedProfileName
    {
        get;
        set
        {
            var sanitized = SanitizeFileName(value);
            if (field == sanitized) return;
            field = sanitized;
            OnPropertyChanged(nameof(CurrentlyLoadedProfileName));
        }
    }

    /// <summary>
    /// Stores the directory path of the currently loaded profile, if it was loaded from a specific file.
    /// </summary>
    private string? _currentlyLoadedProfileDirectory;

    /// <summary>
    /// Gets or sets the game profile associated with the current instance.
    /// </summary>
    public T GameProfile { get; } = new();

    /// <summary>
    /// Represents the default name assigned to a newly created profile.
    /// </summary>
    public const string NewProfileName = "New Profile";

    /// <summary>
    /// Represents the file extension used for JSON files.
    /// </summary>
    private const string JsonFileExtension = ".json";

    /// <summary>
    /// Provides default options for JSON serialization.
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    #region ENCRYPTION

    /// <summary>
    /// Gets or sets the AES encryption utility used for cryptographic operations.
    /// </summary>
    private AesCrypto? Encryptor { get; set; }

    /// <summary>
    /// Sets the encryptor.
    /// </summary>
    /// <param name="key">The encryption key to use for initializing the encryptor.</param>
    public void SetEncryptor(string key)
        => Encryptor = new AesCrypto(key);

    /// <summary>
    /// Determines whether encryption is currently enabled for this instance.
    /// </summary>
    /// <returns><see langword="true"/> if encryption is enabled; otherwise, <see langword="false"/>.</returns>
    public bool EncryptionEnabled() => Encryptor != null;

    /// <summary>
    /// Represents the file extension used for encrypted files.
    /// </summary>
    private const string EncryptedFileExtension = ".bin";
    
    #endregion

    /// <summary>
    /// Returns the directory path currently associated with the loaded profile, creating the directory if it does not already exist.
    /// </summary>
    /// <returns>A string containing the full path of the directory for the currently loaded profile.</returns>
    public string GetDirectory()
    {
        var dir = _currentlyLoadedProfileDirectory ?? _gpDirectory;
        Directory.CreateDirectory(dir);
        return dir;
    }

    /// <summary>
    /// Constructs the full file path for the currently loaded profile, selecting the appropriate file extension based on whether encryption is enabled.
    /// </summary>
    /// <returns>A string containing the absolute path to the profile file, with either the encrypted or JSON file extension applied.</returns>
    public string GetFilePath()
    {
        var extension = Encryptor != null ? EncryptedFileExtension : JsonFileExtension;
        return Path.Combine(GetDirectory(), $"{CurrentlyLoadedProfileName}{extension}");
    }

    /// <summary>
    /// Creates and loads a new, empty game profile, replacing any currently loaded profile.
    /// </summary>
    public void NewProfile() 
        => Load(new T(), NewProfileName);

    /// <summary>
    /// Loads the specified game profile and sets the current profile name.
    /// </summary>
    /// <param name="gameProfile">The game profile to be loaded.</param>
    /// <param name="profileName">The name to associate with the loaded profile.</param>
    public void Load(T gameProfile, string profileName)
    {
        GameProfile.Set(gameProfile);
        CurrentlyLoadedProfileName = profileName;
        _currentlyLoadedProfileDirectory = null;
    }

    /// <summary>
    /// Loads a game profile from the specified file and sets it as the current profile.
    /// </summary>
    /// <param name="filePath">The path to the game profile file to load. Must refer to an existing file containing a valid profile in the expected format.</param>
    /// <exception cref="FileNotFoundException">Thrown if the file specified by <paramref name="filePath"/> does not exist.</exception>
    /// <exception cref="Exception">Thrown if the file cannot be deserialized into a valid game profile, or if the profile type in the file does not match the expected type.</exception>
    public void Load(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("Game Profile file not found.", filePath);
        var data = File.ReadAllText(filePath);
        if (Encryptor != null) data = Encryptor.Decrypt(data);
        var newGp = JsonSerializer.Deserialize<T>(data, _jsonOptions) ?? throw new Exception("Failed to deserialize game profile.");
        ApplyLoadedProfile(newGp, Path.GetFileNameWithoutExtension(filePath), Path.GetDirectoryName(filePath));
    }

    /// <summary>
    /// Loads a game profile from the specified JSON data and applies it under the given profile name.
    /// </summary>
    /// <param name="data">A JSON-formatted string containing the serialized game profile data to load. If encryption is enabled, this data should be encrypted and will be decrypted automatically.</param>
    /// <param name="profileName">The name to assign to the loaded profile. This determines how the profile is identified within the application.</param>
    /// <exception cref="Exception">Thrown if the provided data cannot be deserialized into a valid game profile.</exception>
    public void Load(string data, string profileName)
    {
        if (Encryptor != null) data = Encryptor.Decrypt(data);
        var newGp = JsonSerializer.Deserialize<T>(data, _jsonOptions) ?? throw new Exception("Failed to deserialize game profile.");
        ApplyLoadedProfile(newGp, profileName, null);
    }

    /// <summary>
    /// Applies common validation and state updates for a loaded game profile instance.
    /// </summary>
    /// <param name="newGp">Deserialized game profile instance to validate and apply.</param>
    /// <param name="profileName">Name to set for the loaded profile.</param>
    /// <param name="directory">Directory where the profile was loaded from, or null if not loaded from file.</param>
    private void ApplyLoadedProfile(T newGp, string profileName, string? directory)
    {
        var defaultGp = new T();
        if (newGp.Meta.GpType != defaultGp.Meta.GpType)
            throw new Exception($"Game Profile file type '{newGp.Meta.GpType}' does not match the expected type '{defaultGp.Meta.GpType}'.");

        GameProfile.Set(newGp);
        CurrentlyLoadedProfileName = profileName;
        _currentlyLoadedProfileDirectory = directory;
    }

    /// <summary>
    /// Prepares a serialized representation of the current game profile and outputs it as a string, optionally encrypting the result if an encryptor is available.
    /// </summary>
    /// <param name="data">When this method returns, contains the prepared data as a JSON string. If an encryptor is set, the string will be encrypted; otherwise, it will be plain JSON.</param>
    public void PrepareData(out string data)
    {
        // prepare the data
        data = JsonSerializer.Serialize(GameProfile, _jsonOptions);
        // save the data
        if (Encryptor != null) data = Encryptor.Encrypt(data);
    }

    /// <summary>
    /// Saves the current game profile to the specified file path, optionally encrypting the data before writing.
    /// </summary>
    /// <param name="filePath">The path of the file to which the game profile will be saved.</param>
    public void Save(string? filePath = null)
    {
        PrepareData(out var data);
        filePath ??= GetFilePath();
        File.WriteAllText(filePath, data);
    }
    
    /// <summary>
    /// Removes invalid characters from a file name and trims any trailing spaces or periods.
    /// </summary>
    /// <param name="fileName">The file name to sanitize.</param>
    /// <returns>A sanitized version of the file name with invalid characters removed and trailing spaces or periods trimmed or null if the input is null.</returns>
    private static string? SanitizeFileName(string? fileName)
    {
        if (fileName is null) return null;
        var invalid = Path.GetInvalidFileNameChars();
        var sb = new StringBuilder(fileName.Length);
        foreach (var c in from c in fileName let isInvalid = invalid.Any(ic => c == ic) where !isInvalid select c)
            sb.Append(c);
        var result = sb.ToString();
        result = result.TrimEnd(' ', '.');
        return result;
    }

    // MVVM support
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) 
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}