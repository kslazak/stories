using StoriesAPI.Models;

namespace StoriesAPI;

/// <summary>
/// Defines methods for story handling logic.
/// </summary>
public interface IStoryService
{
    /// <summary>
    /// Retrieves the specified number of best story Ids (either from cache or from the Hacker News API).
    /// </summary>
    /// <param name="count">Number of Ids to retrieve.</param>
    /// <returns>Retrieved Ids.</returns>
    List<int> GetBestStoryIds(int count);

    /// <summary>
    /// Retrieves details of specified stories (either from cache or from the Hacker News API).
    /// </summary>
    /// <param name="ids">Ids of the stories to retrieved.</param>
    /// <returns>Retrieved stories.</returns>
    IEnumerable<Story> GetStories(List<int> ids);
}
