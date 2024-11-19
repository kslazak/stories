namespace StoriesAPI.Models;

/// <summary>
/// Represents a story returned by the Hacker News API.
/// </summary>
public class ExternalStory
{
    /// <summary>
    /// Story title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Story URL.
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// Name of the user this story has been posted by.
    /// </summary>
    public string? By { get; set; }

    /// <summary>
    /// Date and time this story was posted (in Unix time format).
    /// </summary>
    public long Time { get; set; }

    /// <summary>
    /// Story score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Total number of comments.
    /// </summary>
    public int Descendants { get; set; }
}