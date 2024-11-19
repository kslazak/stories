using StoriesAPI.Models;

namespace StoriesUnitTests;

[TestFixture]
public class StoryTest
{
    [TestCase("Title", "Title")]
    [TestCase("Longer title", "Longer title")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void ConstructorShouldPopulateTitleProperly(string? sourceTitle, string expectedTitle)
    {
        var externalStory = new ExternalStory { Title = sourceTitle };
        
        var result = new Story(externalStory);

        Assert.That(result.Title == expectedTitle);
    }

    [TestCase("http://www.test.org", "http://www.test.org")]
    [TestCase("https://www.test.org:443/articles/new/234436", "https://www.test.org:443/articles/new/234436")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void ConstructorShouldPopulateUriProperly(string? sourceUrl, string expectedUri)
    {
        var externalStory = new ExternalStory { Url = sourceUrl };

        var result = new Story(externalStory);

        Assert.That(result.Uri == expectedUri);
    }

    [TestCase("user", "user")]
    [TestCase("User Name", "User Name")]
    [TestCase("", "")]
    [TestCase(null, "")]
    public void ConstructorShouldPopulatePostedByProperly(string? sourceBy, string expectedPostedBy)
    {
        var externalStory = new ExternalStory { By = sourceBy };

        var result = new Story(externalStory);

        Assert.That(result.PostedBy == expectedPostedBy);
    }

    [TestCase(1, "1970-01-01T00:00:01+00:00")]
    [TestCase(1732027474, "2024-11-19T14:44:34+00:00")]
    public void ConstructorShouldPopulateTimeProperly(long sourceTime, string expectedTime)
    {
        var externalStory = new ExternalStory { Time = sourceTime };

        var result = new Story(externalStory);

        Assert.That(result.Time == expectedTime);
    }

    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(int.MaxValue, int.MaxValue)]
    public void ConstructorShouldPopulateScoreProperly(int sourceScore, int expectedScore)
    {
        var externalStory = new ExternalStory { Score = sourceScore };

        var result = new Story(externalStory);

        Assert.That(result.Score == expectedScore);
    }

    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(int.MaxValue, int.MaxValue)]
    public void ConstructorShouldPopulateCommentCountProperly(int sourceDescendants, int expectedCommentCount)
    {
        var externalStory = new ExternalStory { Descendants = sourceDescendants };

        var result = new Story(externalStory);

        Assert.That(result.CommentCount == expectedCommentCount);
    }
}