using StoriesAPI.Models;

namespace StoriesAPI;

/// <summary>
/// Item that can be stored in the cache.
/// </summary>
public class CacheItem(Story value)
{
    /// <summary>
    /// Gets the value of cached item.
    /// </summary>
    public Story Value { get; } = value;

    /// <summary>
    /// Gets the time when cached item was created.
    /// </summary>
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}
