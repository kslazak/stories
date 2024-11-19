using StoriesAPI;

namespace StoriesUnitTests;

[TestFixture]
public class CacheItemTest
{
    [Test]
    public void ConstructorShouldPopulateValueProperly()
    {
        var story = TestUtils.CreateStory();
        
        var result = new CacheItem(story);

        Assert.That(result.Value == story);
    }

    [Test]
    public void ConstructorShouldPopulateCreatedAtProperly()
    {
        var story = TestUtils.CreateStory();
        var now = DateTime.UtcNow;

        var result = new CacheItem(story);

        Assert.That(result.CreatedAt - now < TimeSpan.FromSeconds(1));
    }
}