using RestSharp;
using StoriesAPI.Models;

namespace StoriesAPI;

/// <summary>
/// Provides story data from the Hacker News API.
/// </summary>
public class StoryProvider(ILogger<StoryCache> logger, IHttpContextAccessor httpContextAccessor) : IStoryProvider
{
    private readonly ILogger<StoryCache> _logger = logger;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly RestClient _client = new(new RestClientOptions("https://hacker-news.firebaseio.com/v0"));

    /// <summary>
    /// Retrieves the Ids of the best stories from the Hacker News API.
    /// </summary>
    /// <returns>Retrieved Ids.</returns>
    public async Task<List<int>> GetBestStoryIds()
    {
        try
        {
            _logger.LogAsInformation("Getting best story Ids from the Hacker News API.",
                _httpContextAccessor.HttpContext);

            var request = new RestRequest("beststories.json");
            return await _client.GetAsync<List<int>>(request) ?? [];
        }
        catch (Exception exception)
        {
            _logger.LogAsError(exception.Message, _httpContextAccessor.HttpContext);
            return [];
        }
        finally
        {
            _logger.LogAsInformation("Completed getting best story Ids from the Hacker News API.",
                _httpContextAccessor.HttpContext);
        }
    }

    /// <summary>
    /// Retrieves a story with the specified Id from the Hacker News API.
    /// </summary>
    /// <param name="storyId">Id of the story to retrieve.</param>
    /// <returns>Retrieved story or null if specified story could not be retrieved.</returns>
    public async Task<Story?> GetStory(int storyId)
    {
        try
        {
            _logger.LogAsInformation($"Getting story {storyId} from the Hacker News API.",
                _httpContextAccessor.HttpContext);

            var request = new RestRequest($"item/{storyId}.json");
            var result = await _client.GetAsync<ExternalStory>(request);
            return result == null ? null : new Story(result);
        }
        catch (Exception exception)
        {
            _logger.LogAsError(exception.Message, _httpContextAccessor.HttpContext);
            return null;
        }
        finally
        {
            _logger.LogAsInformation($"Completed getting story {storyId} from the Hacker News API.",
                _httpContextAccessor.HttpContext);
        }
    }
}
