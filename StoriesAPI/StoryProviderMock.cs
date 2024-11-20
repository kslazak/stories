using StoriesAPI.Models;

namespace StoriesAPI;

/// <summary>
/// Mocks the story retrieval.
/// </summary>
public class StoryProviderMock : IStoryProvider
{
    private readonly List<int> _storyIds;
    private readonly Dictionary<int, Story?> _stories;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public StoryProviderMock()
    {
        _storyIds = Enumerable.Range(1, 200).Reverse().ToList();
        _stories = _storyIds.ToDictionary(id => id, CreateStory);
    }

    /// <summary>
    /// Retrieves the predefined Ids of the best stories.
    /// </summary>
    /// <returns>Retrieved Ids.</returns>
    public Task<List<int>> GetBestStoryIds()
    {
         return Task.FromResult(_storyIds);
    }

    /// <summary>
    /// Retrieves a predefined story for the specified Id.
    /// </summary>
    /// <param name="storyId">Id of the story to retrieve.</param>
    /// <returns>Retrieved story or null if specified story could not be retrieved.</returns>
    public Task<Story?> GetStory(int storyId)
    {
        return Task.FromResult(_stories.TryGetValue(storyId, out var value) ? value : null);
    }

    private static Story? CreateStory(int storyId)
    {
        return new Story
        {
            Title = $"Title {storyId}",
            Uri = $"Uri {storyId}",
            PostedBy = $"User {storyId}",
            Time = DateTime.UnixEpoch.AddSeconds(storyId).ToString("yyyy-MM-ddTHH:mm:sszzz"),
            Score = storyId,
            CommentCount = storyId
        };
    }
}
