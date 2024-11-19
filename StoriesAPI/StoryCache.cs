using System.Collections.Concurrent;
using StoriesAPI.Models;

namespace StoriesAPI;

/// <summary>
/// Provides story cache with configurable retention period.
/// </summary>
public class StoryCache(ILogger<StoryCache> logger, IConfigurationProvider configurationProvider,
    IHttpContextAccessor httpContextAccessor) : IStoryCache
{
    private const int InfiniteRetention = -1;

    private readonly ILogger<StoryCache> _logger = logger;
    private readonly IConfigurationProvider _configurationProvider = configurationProvider;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly object _configLock = new();
    private readonly ReaderWriterLockSlim _listLock = new();
    private readonly List<int> _idList = new(200);
    private readonly ConcurrentDictionary<int, CacheItem> _stories = new(Environment.ProcessorCount * 4, 400);

    private int? _retentionSeconds;
    private DateTime _listUpdateTime = DateTime.MinValue;

    /// <summary>
    /// Gets the specified number of Ids of the best stories.
    /// </summary>
    /// <param name="count">Number of Ids to retrieve.</param>
    /// <returns>Ids of the best stories or null if cache has expired or contains less items than requested.</returns>
    public List<int>? GetBestStoryIds(int count)
    {
        _listLock.EnterReadLock();
        try
        {
            if (IsListExpired)
            {
                _logger.LogAsInformation("Cached list of Ids has expired.", _httpContextAccessor.HttpContext);
                return null;
            }

            if (_idList.Count < count)
            {
                _logger.LogAsWarning($"{count} Ids requested but only {_idList.Count} Ids found in cache.", _httpContextAccessor.HttpContext);
                return null;
            }

            _logger.LogAsInformation("Found requested number of Ids in cache.", _httpContextAccessor.HttpContext);
            return _idList[..count];
        }
        finally
        {
            _listLock.ExitReadLock();
        }
    }

    /// <summary>
    /// Sets Ids of the best stories.
    /// </summary>
    /// <param name="ids">Best story Ids to be cached.</param>
    public void SetBestStoryIds(IEnumerable<int> ids)
    {
        _logger.LogAsInformation("Storing best story Ids in cache.", _httpContextAccessor.HttpContext);

        _listLock.EnterWriteLock();
        try
        {
            _idList.Clear();
            _idList.AddRange(ids);
            _listUpdateTime = DateTime.UtcNow;
        }
        finally
        {
            _listLock.ExitWriteLock();
            _logger.LogAsInformation("Best story Ids stored in cache.", _httpContextAccessor.HttpContext);
        }
    }

    /// <summary>
    /// Retrieves a story with the specified Id.
    /// </summary>
    /// <param name="id">Id of the story to retrieve.</param>
    /// <returns>Retrieved story or null if story does not exist in cache or it has expired.</returns>
    public Story? GetStory(int id)
    {
        if (!_stories.TryGetValue(id, out var item))
        {
            _logger.LogAsInformation($"Story {id} was not found in cache.", _httpContextAccessor.HttpContext);
            return null;
        }
        if (IsItemExpired(item))
        {
            _logger.LogAsInformation($"Cached story {id} has expired.", _httpContextAccessor.HttpContext);
            return null;
        }

        _logger.LogAsInformation($"Found story {id} in cache.", _httpContextAccessor.HttpContext);
        return item.Value;
    }

    /// <summary>
    /// Adds or updates a story with the specified Id.
    /// </summary>
    /// <param name="id">Id of the story to add or update.</param>
    /// <param name="story">Story to be added or updated.</param>
    public void AddOrUpdateStory(int id, Story story)
    {
        _logger.LogAsInformation($"Storing story {id} in cache.", _httpContextAccessor.HttpContext);

        _stories[id] = new CacheItem(story);

        _logger.LogAsInformation($"Story {id} stored in cache.", _httpContextAccessor.HttpContext);
    }

    private bool IsListExpired => RetentionSeconds != InfiniteRetention &&
                                  _listUpdateTime.AddSeconds(RetentionSeconds) < DateTime.UtcNow;

    private int RetentionSeconds
    {
        get
        {
            if (_retentionSeconds == null)
            {
                lock (_configLock)
                {
                    _retentionSeconds ??= GetCacheRetentionFromConfig();
                }
            }

            return _retentionSeconds.Value;
        }
    }

    private bool IsItemExpired(CacheItem item)
    {
        return RetentionSeconds != InfiniteRetention &&
               item.CreatedAt.AddSeconds(RetentionSeconds) < DateTime.UtcNow;
    }

    private int GetCacheRetentionFromConfig()
    {
        var defaultValueDescription = $"Assuming the default value of {InfiniteRetention} (infinite retention).";
        var configuredValue = _configurationProvider.GetCacheRetentionSeconds();
        if (configuredValue == null)
        {
            _logger.LogAsWarning($"CacheRetentionSeconds not found in config file. {defaultValueDescription}",
                _httpContextAccessor.HttpContext);
            return InfiniteRetention;
        }

        if (!int.TryParse(configuredValue, out var parsedValue) || parsedValue < -1)
        {
            _logger.LogAsWarning(
                $"Incorrect CacheRetentionSeconds value in config file. It should be an integer greater than or equal to -1. {defaultValueDescription}",
                _httpContextAccessor.HttpContext);
            return InfiniteRetention;
        }

        var retention = parsedValue == InfiniteRetention ? "infinity" : $"{parsedValue} seconds";
        _logger.LogAsInformation($"Loaded CacheRetentionSeconds setting from config file. Setting cache retention to {retention}.",
            _httpContextAccessor.HttpContext);
        return parsedValue;
    }
}
