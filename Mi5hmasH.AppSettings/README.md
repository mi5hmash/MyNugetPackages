# Disclaimer
This NuGet package was created exclusively for use in my own open‑source projects published on my GitHub profile.  
While it is publicly available, it is not intended to serve as a general‑purpose library, nor is it designed or documented for external production use.  
Feel free to explore the code, but please keep in mind that its primary purpose is to support my personal project ecosystem.  
  
# Usage
## 1. Creating the MyAppSettings model
```csharp
public class MyAppSettings : IEquatable<MyAppSettings>
{
    //// YOUR APP SETTINGS
    public string String1 { get; set; } = "STRING_1";
    
    public class InnerSectionModel
    {
        public int NumberPositive { get; set; } = 1;
        public int NumberNegative { get; set; } = -2;
    }
    public InnerSectionModel InnerSection { get; set; } = new();
    
    public bool BoolTrue { get; set; } = true;

    //// Equality boilerplate: defines value-based comparison for MyAppSettings
    public bool Equals(MyAppSettings? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;

        var sc = StringComparer.Ordinal;
        return sc.Equals(String1, other.String1) &&
               BoolTrue == other.BoolTrue &&
               InnerSection.NumberPositive == other.InnerSection.NumberPositive &&
               InnerSection.NumberNegative == other.InnerSection.NumberNegative;
    }

    public int GetHashCodeStable()
    {
        var hc = new HashCode();
        var sc = StringComparer.Ordinal;
        // Add fields to the hash code computation
        hc.Add(String1, sc);
        hc.Add(BoolTrue);
        hc.Add(InnerSection.NumberPositive);
        hc.Add(InnerSection.NumberNegative);
        return hc.ToHashCode();
    }
    
    // This is a workaround to avoid the default GetHashCode() implementation in objects where all fields are mutable.
    private readonly Guid _uniqueId = Guid.NewGuid();
    public override int GetHashCode() 
        => _uniqueId.GetHashCode();
    
    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is MyAppSettings castedObj && Equals(castedObj);

    public static bool operator ==(MyAppSettings? left, MyAppSettings? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(MyAppSettings? left, MyAppSettings? right)
        => !(left == right);
}
```

## 2. Loading and saving the settings file
```csharp
// Create an instance of the MyAppSettings with the default values.
var myAppSettings = new MyAppSettings();
// Create an instance of the AppSettingsManager with the MyAppSettings type and pick a flavor for the serializer (in this case, Json).
var appSettingsManager = new AppSettingsManager<MyAppSettings, Json>();
// (OPTIONAL) Set the encryption key if you want to encrypt the settings file. 
// The string should be base64‑encoded 32‑byte (256‑bit) AES encryption key.
private const string EncryptionKeyBase64 = "hLR2ymI3kEMLQ6WrghQ741axGb6xdyMDdRbqR8b4KN4=";
appSettingsManager.SetEncryptor(EncryptionKeyBase64);
// Load the settings with the default values.
appSettingsManager.Load(myAppSettings);
// Save the default values to the settings file. 
// An appsettings.json (or .bin if using encryption) file will be created in the same directory as the executable.
appSettingsManager.Save();

// On the next run you can load the saved file with this command.
appSettingsManager.Load();
```