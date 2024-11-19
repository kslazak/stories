using StoriesAPI.Models;

namespace StoriesAPI;

/// <summary>
/// Defines methods and properties for handling story cache.
/// </summary>
public interface IStoryCache
{
    /// <summary>
    /// Gets the specified number of Ids of the best stories.
    /// </summary>
    /// <param name="count">Number of Ids to retrieve.</param>
    /// <returns>Ids of the best stories or null if cache has expired or contains less items than requested.</returns>
    List<int>? GetBestStoryIds(int count);

    /// <summary>
    /// Sets Ids of the best stories.
    /// </summary>
    /// <param name="ids">Best story Ids to be cached.</param>
    void SetBestStoryIds(IEnumerable<int> ids);

    /// <summary>
    /// Retrieves a story with the specified Id.
    /// </summary>
    /// <param name="id">Id of the story to retrieve.</param>
    /// <returns>Retrieved story or null if story does not exist in cache or it has expired.</returns>
    Story? GetStory(int id);

    /// <summary>
    /// Adds or updates a story with the specified Id.
    /// </summary>
    /// <param name="id">Id of the story to add or update.</param>
    /// <param name="story">Story to be added or updated.</param>
    void AddOrUpdateStory(int id, Story story);
}
