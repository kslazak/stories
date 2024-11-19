using StoriesAPI.Models;

namespace StoriesAPI;

/// <summary>
/// Defines methods for story provider.
/// </summary>
public interface IStoryProvider
{
    /// <summary>
    /// Retrieves the Ids of the best stories from the Hacker News API.
    /// </summary>
    /// <returns>Retrieved Ids.</returns>
    Task<List<int>> GetBestStoryIds();

    /// <summary>
    /// Retrieves a story with the specified Id from the Hacker News API.
    /// </summary>
    /// <param name="storyId">Id of the story to retrieve.</param>
    /// <returns>Retrieved story or null if specified story could not be retrieved.</returns>
    Task<Story?> GetStory(int storyId);
}
