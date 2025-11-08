using Mi5hmasH.GameProfile;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace QualityControl.xUnit;

public class TestGameProfile : IEquatable<TestGameProfile>, INotifyPropertyChanged, IGameProfile
{
    /// <summary>
    /// Specifies the supported gaming platforms for the application.
    /// </summary>
    public enum GamingPlatform
    {
        Other = 0,
        Steam = 1,
        EpicGames = 2,
        GOG = 3,
        Origin = 4,
        Uplay = 5,
        XboxOne = 100,
        XboxSeriesX = 101,
        PS4 = 150,
        PS5 = 151,
        NintendoSwitch = 200
    }

    /// <summary>
    /// Gets or sets metadata information related to the game profile.
    /// </summary>
    public GameProfileMeta Meta { get; set; } = new("TestProfile", new Version(1, 0, 0, 0));

    /// <summary>
    /// Gets or sets the title of a game.
    /// </summary>
    public string? GameTitle
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged(nameof(GameTitle));
        }
    }

    /// <summary>
    /// Gets or sets the GamingPlatform.
    /// </summary>
    public GamingPlatform Platform
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged(nameof(Platform));
        }
    } = GamingPlatform.Other;

    /// <summary>
    /// Gets or sets the application identifier associated with this instance.
    /// </summary>
    public string? AppId
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged(nameof(AppId));
        }
    }

    /// <summary>
    /// Copies the game profile data from the specified object if it is an instance of TestGameProfile.
    /// </summary>
    /// <param name="other">The object from which to copy game profile data.</param>
    public void Set(object other)
    {
        if (other is not TestGameProfile profile) return;
        GameTitle = profile.GameTitle;
        AppId = profile.AppId;
        Platform = profile.Platform;
    }

    public bool Equals(TestGameProfile? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;

        var sc = StringComparer.Ordinal;
        return Meta.Equals(other.Meta) &&
               sc.Equals(GameTitle, other.GameTitle) &&
               sc.Equals(AppId, other.AppId) &&
               Platform == other.Platform;
    }

    public int GetHashCodeStable()
    {
        var hc = new HashCode();
        var sc = StringComparer.Ordinal;
        // Add fields to the hash code computation
        hc.Add(Meta);
        hc.Add(GameTitle, sc);
        hc.Add(AppId, sc);
        hc.Add(Platform);
        return hc.ToHashCode();
    }

    // This is a workaround to avoid the default GetHashCode() implementation in objects where all fields are mutable.
    private readonly Guid _uniqueId = Guid.NewGuid();

    public override int GetHashCode()
        => _uniqueId.GetHashCode();

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is TestGameProfile castedObj && Equals(castedObj);

    public static bool operator ==(TestGameProfile? left, TestGameProfile? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(TestGameProfile? left, TestGameProfile? right)
        => !(left == right);

    // MVVM support
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed class GameProfileTests : IDisposable
{
    private readonly GameProfileManager<TestGameProfile> _gameProfileManager = new();
    private const string EncKey = "Si5JmNcqQkmmK+DLCfy6pYUnn7dFUCBfFf/FUPZAzp0=";
    private readonly ITestOutputHelper _output;

    public GameProfileTests(ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("SETUP");
    }

    public void Dispose()
    {
        _output.WriteLine("CLEANUP");
    }
    
    private void LoadGameProfile()
    {
        // Load GameProfile
        _gameProfileManager.SetEncryptor(EncKey);
        const string gp =
            "RzzQYWP+LCjGv1lbklplByfjrO72dPhocU2U5zhkVMV4kYbaQnxUfsCqHEmlyZpBXDWUEHeBcrVTJbbrMaEqoa/x6biqfQgoE3U1lledi5+tGTuEByFpxTJfkstt8+A2G+rrGpw28fVhu4gtWKM/7AfXEoUHtFMVtppWnmkKhGnROuWQA8mtg81J2Fs6XwXpRzkO7Vwg5oF5E5G4RFTR2kvp6+sV9wrWsOdyRtZhAWTGQfZF6PFU/tx3aBPk2YLK0fUCFNlVsd5VIbPLVox3r7a4BWn8LRamuG5BnTPzShg=";
        _gameProfileManager.Load(gp, "profile");
    }

    [Fact]
    public void Gp_LoadGameProfile_DoesNotThrow()
    {
        // Act
        LoadGameProfile();

        // Assert
        Assert.True(true);
    }

#if DEBUG
    [Fact]
    public void Debug_CreateGameProfile_ResultShouldNotBeNull()
    {
        // Arrange
        _gameProfileManager.SetEncryptor(EncKey);

        // Act
        var gp = new TestGameProfile
        {
            Meta = new GameProfileMeta(),
            GameTitle = "TestTitle",
            Platform = TestGameProfile.GamingPlatform.Other,
            AppId = "777"
        };
        _gameProfileManager.Load(gp, "TestProfile");
        _gameProfileManager.PrepareData(out var result);
        _output.WriteLine($"Game Profile Data: {result}");

        // Assert
        Assert.NotNull(result);
    }
#endif
}