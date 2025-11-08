extern alias AppSettingsAlias;
using AppSettingsAlias::Mi5hmasH.AppSettings;
using AppSettingsAlias::Mi5hmasH.AppSettings.Flavors;
using System.Diagnostics.CodeAnalysis;
using AesCrypto = Mi5hmasH.Utilities.AesCrypto;

namespace QualityControl.xUnit;

#region TEST_MODELS

public class MyAppSettings : IEquatable<MyAppSettings>
{
    public class InnerSectionModel
    {
        public int NumberPositive { get; set; } = 1;
        public int NumberNegative { get; set; } = -2;
    }

    public string String1 { get; set; } = "STRING_1";
    public InnerSectionModel InnerSection { get; set; } = new();
    public bool BoolTrue { get; set; } = true;

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

#endregion


#region TESTS

public sealed class AppSettingsTests : IDisposable
{
    private readonly ITestOutputHelper _output;

    public AppSettingsTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }

    private const string EncryptionKeyBase64 = "hLR2ymI3kEMLQ6WrghQ741axGb6xdyMDdRbqR8b4KN4=";
    
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Json_Create_ShouldSaveAndLoadCorrectly(bool doesEncrypt)
    {
        // Arrange
        var myAppSettings = new MyAppSettings();
        var appSettingsManager = new AppSettingsManager<MyAppSettings, Json>();
        if (doesEncrypt) appSettingsManager.SetEncryptor(EncryptionKeyBase64);
        appSettingsManager.Load(myAppSettings);

        // Act
        appSettingsManager.Save();
        appSettingsManager.Load();

        // Assert
        Assert.Equal(myAppSettings, appSettingsManager.Settings);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Xml_Create_ShouldSaveAndLoadCorrectly(bool doesEncrypt)
    {
        // Arrange
        var myAppSettings = new MyAppSettings();
        var appSettingsManager = new AppSettingsManager<MyAppSettings, Xml>();
        if (doesEncrypt) appSettingsManager.SetEncryptor(EncryptionKeyBase64);
        appSettingsManager.Load(myAppSettings);

        // Act
        appSettingsManager.Save();
        appSettingsManager.Load();

        // Assert
        Assert.Equal(myAppSettings, appSettingsManager.Settings);
    }

    [Theory]
    [InlineData("It's-a Me, Mario!")]
    [InlineData("Zażółć gęślą jaźń!")]
    public void AesCrypto_StringEncryption_ShouldEncryptAndDecryptCorrectly(string text)
    {
        // Arrange
        const string key = "PsJJ0bpcv3hOACfIjqPT1xWfIVGOUniTnOlJKtzKrjQ=";
        var encryptor = new AesCrypto(key);

        // Act
        var encryptedString = encryptor.Encrypt(text);
        var decryptedString = encryptor.Decrypt(encryptedString);

        // Assert
        Assert.Equal(text, decryptedString);
    }

#if DEBUG
    [Fact]
    public void Debug_GenerateKey_ResultShouldNotBeNull()
    {
        // Act
        var result = AesCrypto.Debug_GenerateKey();
        _output.WriteLine($"Generated Key: {result}");

        // Assert
        Assert.NotNull(result);
    }
#endif
}

#endregion