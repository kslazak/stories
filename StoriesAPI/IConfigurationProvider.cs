namespace StoriesAPI;

/// <summary>
/// Defines methods for configuration provider.
/// </summary>
public interface IConfigurationProvider
{
    /// <summary>
    /// Retrieves the CacheRetentionSeconds setting from the configuration file.
    /// </summary>
    /// <returns>Retrieved CacheRetentionSeconds value.</returns>
    string? GetCacheRetentionSeconds();
}
