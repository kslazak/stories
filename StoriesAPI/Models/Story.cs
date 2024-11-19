namespace StoriesAPI.Models;

/// <summary>
/// Represents a story.
/// </summary>
public class Story(ExternalStory story)
{
    /// <summary>
    /// Gets story title.
    /// </summary>
    public string Title { get; } = story.Title ?? string.Empty;

    /// <summary>
    /// Gets story URI.
    /// </summary>
    public string Uri { get; } = story.Url ?? string.Empty;

    /// <summary>
    /// Gets the name of the user this story has been posted by.
    /// </summary>
    public string PostedBy { get; } = story.By ?? string.Empty;

    /// <summary>
    /// Gets date and time this story was posted.
    /// </summary>
    public string Time { get; } = DateTime.UnixEpoch.AddSeconds(story.Time).ToString("yyyy-MM-ddTHH:mm:sszzz");

    /// <summary>
    /// Gets story score.
    /// </summary>
    public int Score { get; } = story.Score;

    /// <summary>
    /// Gets total number of comments for this story.
    /// </summary>
    public int CommentCount { get; } = story.Descendants;
}