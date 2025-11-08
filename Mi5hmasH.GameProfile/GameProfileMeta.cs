using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Mi5hmasH.GameProfile;

/// <summary>
/// The Meta section of the <see cref="IGameProfile"/> file.
/// </summary>
public class GameProfileMeta(string? gpType = null, Version? gpVersion = null) : IEquatable<GameProfileMeta>, INotifyPropertyChanged
{
    /// <summary>
    /// Game Profile type.
    /// </summary>
    public string GpType
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged(nameof(GpType));
        }
    } = gpType ?? string.Empty;

    /// <summary>
    /// Game Profile version.
    /// </summary>
    public Version GpVersion
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            OnPropertyChanged(nameof(GpVersion));
        }
    } = gpVersion ?? new Version(1, 0, 0, 0);

    /// <summary>
    /// Copies data from the specified object if it is an instance of GameProfileMeta.
    /// </summary>
    /// <param name="other">The other instance of an object from which to copy data.</param>
    public void Set(object other)
    {
        if (other is not GameProfileMeta meta) return;
        GpType = meta.GpType;
        GpVersion = meta.GpVersion;
    }
    
    public bool Equals(GameProfileMeta? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;

        var sc = StringComparer.Ordinal;
        return sc.Equals(GpType, other.GpType) &&
               GpVersion == other.GpVersion;
    }

    public int GetHashCodeStable()
    {
        var hc = new HashCode();
        var sc = StringComparer.Ordinal;
        // Add fields to the hash code computation
        hc.Add(GpType, sc);
        hc.Add(GpVersion);
        return hc.ToHashCode();
    }

    // This is a workaround to avoid the default GetHashCode() implementation in objects where all fields are mutable.
    private readonly Guid _uniqueId = Guid.NewGuid();

    public override int GetHashCode()
        => _uniqueId.GetHashCode();

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is GameProfileMeta castedObj && Equals(castedObj);

    public static bool operator ==(GameProfileMeta? left, GameProfileMeta? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(GameProfileMeta? left, GameProfileMeta? right)
        => !(left == right);

    // MVVM support
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}