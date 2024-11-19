using StoriesAPI.Models;
using System.Collections.Concurrent;

namespace StoriesAPI;

/// <summary>
/// Defines story handling logic.
/// </summary>
public class StoryService(ILogger<StoryService> logger, IStoryProvider storyProvider, IStoryCache storyCache,
    IHttpContextAccessor httpContextAccessor) : IStoryService
{
    private readonly ILogger<StoryService> _logger = logger;
    private readonly IStoryProvider _storyProvider = storyProvider;
    private readonly IStoryCache _storyCache = storyCache;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Retrieves the specified number of best story Ids (either from cache or from the Hacker News API).
    /// </summary>
    /// <param name="count">Number of Ids to retrieve.</param>
    /// <returns>Retrieved Ids.</returns>
    public List<int> GetBestStoryIds(int count)
    {
        var ids = _storyCache.GetBestStoryIds(count);
        if (ids == null)
        {
            ids = _storyProvider.GetBestStoryIds().Result;
            var actualCount = ids.Count;
            if (actualCount < count)
                _logger.LogAsWarning($"{count} stories requested but only {actualCount} Ids returned from the API.", _httpContextAccessor.HttpContext);

            _storyCache.SetBestStoryIds(ids);

            if (count < actualCount)
                ids = ids[..count];
        }

        return ids;
    }

    /// <summary>
    /// Retrieves details of specified stories (either from cache or from the Hacker News API).
    /// </summary>
    /// <param name="ids">Ids of the stories to retrieved.</param>
    /// <returns>Retrieved stories.</returns>
    public IEnumerable<Story> GetStories(List<int> ids)
    {
        var storyCount = ids.Count;
        var stories = new ConcurrentDictionary<int, Story>(storyCount, storyCount);
        var tasks = new List<Task>(storyCount);
        var missingStoryIds = new ConcurrentBag<int>();

        foreach (var id in ids)
        {
            var story = _storyCache.GetStory(id);
            if (story != null)
            {
                stories[id] = story;
            }
            else
            {
                tasks.Add(Task.Run(async () =>
                {
                    story = await _storyProvider.GetStory(id);
                    if (story == null)
                    {
                        missingStoryIds.Add(id);
                    }
                    else
                    {
                        _storyCache.AddOrUpdateStory(id, story);
                        stories[id] = story;
                    }
                }));
            }
        }

        Task.WaitAll([.. tasks]);

        return ids.Except(missingStoryIds).Select(id => stories[id]);
    }
}
