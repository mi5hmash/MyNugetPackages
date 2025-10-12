namespace Mi5hmasH.GameProfile;

public interface IGameProfile
{
    /// <summary>
    /// Gets or sets metadata information related to the game profile.
    /// </summary>
    GameProfileMeta Meta { get; set; }

    /// <summary>
    /// Copies the game profile data from the specified object.
    /// </summary>
    /// <param name="other">The object from which to copy game profile data.</param>
    void Set(object other);
}