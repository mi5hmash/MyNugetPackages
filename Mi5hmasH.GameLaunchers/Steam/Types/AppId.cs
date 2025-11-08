using System.Diagnostics.CodeAnalysis;
using Mi5hmasH.Utilities.Helpers;

namespace Mi5hmasH.GameLaunchers.Steam.Types;

/// <summary>
/// Represents a Steam Application Identifier, including its type, ID, title, and developer information.
/// </summary>
public class AppId(uint id, AppId.AppTypeEnum type = AppId.AppTypeEnum.Unknown, string? title = null, string? developer = null) : IEquatable<AppId>
{
    /// <summary>
    /// All known App types.
    /// </summary>
    public enum AppTypeEnum
    {
        Unknown,
        Application,
        Game,
        Demo,
        Dlc,
        Music
    }

    /// <summary>
    /// The type of App.
    /// </summary>
    public AppTypeEnum Type { get; private set; } = type;

    /// <summary>
    /// The App ID.
    /// </summary>
    public uint Id { get; private set; } = id;

    /// <summary>
    /// The title of the App.
    /// </summary>
    public string? Title { get; private set; } = title;

    /// <summary>
    /// The developer of the App.
    /// </summary>
    public string? Developer { get; private set; } = developer;

    /// <summary>
    /// Opens the Steam store page for this App in the default web browser.
    /// </summary>
    public void OpenSteamAppStoreUrl()
    {
        var url = $"https://store.steampowered.com/app/{Id}";
        try { url.OpenUrl(); }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Opens the Steam developer page for this App in the default web browser.
    /// </summary>
    public void OpenSteamDeveloperUrl()
    {
        var url = $"https://store.steampowered.com/developer/{Developer ?? string.Empty}";
        try { url.OpenUrl(); }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Copies the values of all properties from the specified <see cref="AppId"/> instance to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="AppId"/> instance whose property values will be assigned to this instance.</param>
    public void Set(AppId other)
    {
        Id = other.Id;
        Type = other.Type;
        Title = other.Title;
        Developer = other.Developer;
    }

    public bool Equals(AppId? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;

        var sc = StringComparer.Ordinal;
        return Id == other.Id &&
               Type == other.Type &&
               sc.Equals(Title, other.Title) &&
               sc.Equals(Developer, other.Developer);
    }
    
    public int GetHashCodeStable()
    {
        var hc = new HashCode();
        var sc = StringComparer.Ordinal;
        // Add fields to the hash code computation
        hc.Add(Id);
        hc.Add(Type);
        hc.Add(Title, sc);
        hc.Add(Developer, sc);
        return hc.ToHashCode();
    }

    // This is a workaround to avoid the default GetHashCode() implementation in objects where all fields are mutable.
    private readonly Guid _uniqueId = Guid.NewGuid();
    public override int GetHashCode()
        => _uniqueId.GetHashCode();

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is AppId castedObj && Equals(castedObj);

    public static bool operator ==(AppId? left, AppId? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(AppId? left, AppId? right)
        => !(left == right);
}