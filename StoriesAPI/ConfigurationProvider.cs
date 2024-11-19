namespace StoriesAPI;

/// <summary>
/// Exposes the configuration settings.
/// </summary>
public class ConfigurationProvider(IConfiguration configuration) : IConfigurationProvider
{
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Retrieves the CacheRetentionSeconds setting from the configuration file.
    /// </summary>
    /// <returns>Retrieved CacheRetentionSeconds value.</returns>
    public string? GetCacheRetentionSeconds()
    {
        return _configuration["CacheRetentionSeconds"];
    }
}
