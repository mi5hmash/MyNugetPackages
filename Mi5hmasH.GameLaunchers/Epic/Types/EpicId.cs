using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Mi5hmasH.Utilities.Helpers;

namespace Mi5hmasH.GameLaunchers.Epic.Types;

/// <summary>
/// A class representing an Epic ID.
/// </summary>
public class EpicId : IEquatable<EpicId>
{
    private const int Length = 32;
    private readonly string _pattern = $"^[0-9a-fA-F]{{{Length}}}$";

    /// <summary>
    /// The account ID of the user.
    /// </summary>
    public string AccountId { get; private set; } = "00000000000000000000000000000000";

    /// <summary>
    /// Attempts to set the Epic ID if it matches the required pattern.
    /// </summary>
    /// <param name="epicId">The Epic ID to validate and set. Must match the required pattern.</param>
    /// <returns><see langword="true"/> if the Epic ID is valid and successfully set; otherwise, <see langword="false"/>.</returns>
    public bool TrySetEpicId(string epicId)
    {
        epicId = epicId.Trim();
        if (!Regex.IsMatch(epicId, _pattern)) return false;
        AccountId = epicId;
        return true;
    }

    /// <summary>
    /// Converts the <see cref="AccountId"/> string to a byte array representation.
    /// </summary>
    /// <returns>A byte array representing the <see cref="AccountId"/> string.</returns>
    public byte[] GetAsByteArray()
    {
        var bytes = new byte[AccountId.Length];
        for (var i = 0; i < AccountId.Length; i++)
            bytes[i] = (byte)AccountId[i];
        return bytes;
    }

    /// <summary>
    /// Converts the <see cref="AccountId"/> string to an array of bytes encoded as UTF-16 (little-endian).
    /// </summary>
    /// <returns>A byte array containing the UTF-16 encoded representation of the <see cref="AccountId"/> string.</returns>
    public byte[] GetAsWideString()
    {
        var bytes = new byte[AccountId.Length * 2];
        for (var i = 0; i < AccountId.Length; i++)
        {
            var charBytes = BitConverter.GetBytes(AccountId[i]);
            bytes[i * 2] = charBytes[0];
            bytes[i * 2 + 1] = charBytes[1];
        }
        return bytes;
    }

    /// <summary>
    /// Opens the EGS profile URL in the default web browser.
    /// </summary>
    public void OpenEpicProfileUrl()
    {
        var url = $"https://store.epicgames.com/u/{AccountId}";
        try { url.OpenUrl(); }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Sets the current instance's account identifier to match that of the specified <see cref="EpicId"/>.
    /// </summary>
    /// <param name="other">The <see cref="EpicId"/> instance whose account identifier will be copied.</param>
    public void Set(EpicId other)
    {
        AccountId = other.AccountId;
    }

    public bool Equals(EpicId? other)
    {
        if (ReferenceEquals(this, other))
            return true;
        if (other is null)
            return false;

        var sc = StringComparer.Ordinal;
        return sc.Equals(AccountId, other.AccountId);
    }
    
    public int GetHashCodeStable()
    {
        var hc = new HashCode();
        var sc = StringComparer.Ordinal;
        // Add fields to the hash code computation
        hc.Add(AccountId, sc);
        return hc.ToHashCode();
    }

    // This is a workaround to avoid the default GetHashCode() implementation in objects where all fields are mutable.
    private readonly Guid _uniqueId = Guid.NewGuid();
    public override int GetHashCode()
        => _uniqueId.GetHashCode();

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj is EpicId castedObj && Equals(castedObj);

    public static bool operator ==(EpicId? left, EpicId? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(EpicId? left, EpicId? right)
        => !(left == right);
}