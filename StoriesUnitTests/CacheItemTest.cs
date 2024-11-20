using StoriesAPI;
using StoriesAPI.Models;

namespace StoriesUnitTests;

[TestFixture]
public class CacheItemTest
{
    [Test]
    public void ConstructorShouldPopulateValueProperly()
    {
        var story = new Story();
        
        var result = new CacheItem(story);

        Assert.That(result.Value, Is.EqualTo(story));
    }

    [Test]
    public void ConstructorShouldPopulateCreatedAtProperly()
    {
        var story = new Story();
        var now = DateTime.UtcNow;

        var result = new CacheItem(story);

        Assert.That(result.CreatedAt - now, Is.LessThan(TimeSpan.FromSeconds(1)));
    }
}