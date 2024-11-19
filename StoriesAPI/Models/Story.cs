namespace StoriesAPI.Models;

/// <summary>
/// Represents a story.
/// </summary>
public class Story
{
    /// <summary>
    /// Default ctor.
    /// </summary>
    public Story()
    {
        Title = string.Empty;
        Uri = string.Empty;
        PostedBy = string.Empty;
        Time = DateTime.UnixEpoch.AddSeconds(1).ToString("yyyy-MM-ddTHH:mm:sszzz");
        Score = 0;
        CommentCount = 0;

    }

    /// <summary>
    /// Creates a new Story object based on the <see cref="ExternalStory"/> object.
    /// </summary>
    /// <param name="story"><see cref="ExternalStory"/> object to create the Story object from.</param>
    public Story(ExternalStory story)
    {
        Title = story.Title ?? string.Empty;
        Uri = story.Url ?? string.Empty;
        PostedBy = story.By ?? string.Empty;
        Time = DateTime.UnixEpoch.AddSeconds(story.Time).ToString("yyyy-MM-ddTHH:mm:sszzz");
        Score = story.Score;
        CommentCount = story.Descendants;
    }

    /// <summary>
    /// Gets story title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets story URI.
    /// </summary>
    public string Uri { get; set; }

    /// <summary>
    /// Gets the name of the user this story has been posted by.
    /// </summary>
    public string PostedBy { get; set; }

    /// <summary>
    /// Gets date and time this story was posted.
    /// </summary>
    public string Time { get; set; }

    /// <summary>
    /// Gets story score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets total number of comments for this story.
    /// </summary>
    public int CommentCount { get; set; }
}