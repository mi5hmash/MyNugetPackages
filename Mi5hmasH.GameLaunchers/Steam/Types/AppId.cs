using System.Diagnostics.CodeAnalysis;
using Mi5hmasH.GameLaunchers.Helpers;

namespace Mi5hmasH.GameLaunchers.Steam.Types;

/// <summary>
/// Represents a Steam application identifier, including its type, ID, title, and developer information.
/// </summary>
public class AppId
{
    private byte _type;

    /// <summary>
    /// All known app types.
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
    /// The type of app.
    /// </summary>
    public AppTypeEnum Type
    {
        get => (AppTypeEnum)_type;
        set => _type = (byte)value;
    }

    /// <summary>
    /// The app ID.
    /// </summary>
    public uint Id { get; }
    
    /// <summary>
    /// The title of the app.
    /// </summary>
    public string? Title { get; }
    
    /// <summary>
    /// The developer of the app.
    /// </summary>
    public string? Developer { get; }

    public AppId(uint id, AppTypeEnum type = AppTypeEnum.Unknown, string? title = null, string? developer = null)
    {
        Id = id;
        Type = type;
        Title = title;
        Developer = developer;
    }

    /// <summary>
    /// Opens the Steam store page for this app in the default web browser.
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
    /// Opens the Steam developer page for this app in the default web browser.
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
    
    public bool Equals(AppId other)
    {
        return Id == other.Id &&
               _type == other._type &&
               Title == other.Title &&
               Developer == other.Developer;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is AppId castedObj && Equals(castedObj);

    public override int GetHashCode()
        => HashCode.Combine(Id, Type, Title, Developer);

    public static bool operator ==(AppId left, AppId right)
        => left.Equals(right);

    public static bool operator !=(AppId left, AppId right)
        => !(left == right);
}