using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StoriesAPI;
using StoriesAPI.Models;

namespace StoriesUnitTests;

[TestFixture]
public class StoryCacheTest
{
    private ILogger<StoryCache> _logger;
    private IConfigurationProvider _configurationProvider;
    private IHttpContextAccessor _httpContextAccessor;
    private StoryCache _sut;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger<StoryCache>>();
        _configurationProvider = Substitute.For<IConfigurationProvider>();
        _httpContextAccessor = Substitute.For<HttpContextAccessor>();

        _sut = new StoryCache(_logger, _configurationProvider, _httpContextAccessor);
    }

    #region GetBestStoryIds tests

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void GetBestStoryIdsShouldReturnNullWhenNoIdsAreCached(int count)
    {
        var results = _sut.GetBestStoryIds(count);

        Assert.That(results, Is.Null);
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void GetBestStoryIdsShouldReturnNullWhenCachingIsDisabled(int count)
    {
        _configurationProvider.GetCacheRetentionSeconds().Returns("0");
        _sut.SetBestStoryIds(TestUtils.CreateListOfIds(count));

        var results = _sut.GetBestStoryIds(count);

        Assert.That(results, Is.Null);
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void GetBestStoryIdsShouldReturnNullWhenCachedIdsHaveExpired(int count)
    {
        _configurationProvider.GetCacheRetentionSeconds().Returns("1");
        _sut.SetBestStoryIds(TestUtils.CreateListOfIds(count));
        Thread.Sleep(1001);

        var results = _sut.GetBestStoryIds(count);

        Assert.That(results, Is.Null);
    }

    [TestCase(2)]
    [TestCase(100)]
    [TestCase(200)]
    public void GetBestStoryIdsShouldReturnNullWhenLessIdsAreCachedThanRequested(int count)
    {
        _sut.SetBestStoryIds([1]);

        var results = _sut.GetBestStoryIds(count);

        Assert.That(results, Is.Null);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void GetBestStoryIdsShouldReturnRequestedNumberOfIdsWhenNumberOfCachedIdsIsGreaterOrEqualToRequestedCount(int count)
    {
        _sut.SetBestStoryIds([1, 2, 3]);

        var results = _sut.GetBestStoryIds(count);

        Assert.That(results?.Count, Is.EqualTo(count));
    }

    #endregion GetBestStoryIds tests

    #region SetBestStoryIds tests

    [Test]
    public void SetBestStoryIdsShouldUpdateTheCachedIdsWithProvidedValues()
    {
        _sut.SetBestStoryIds([1, 2, 3]);
        _sut.SetBestStoryIds([4, 5, 6]);

        var results = _sut.GetBestStoryIds(3);

        Assert.Multiple(() =>
        {
            Assert.That(results, Is.Not.Null);
            Assert.That(results, Has.Count.EqualTo(3));
            Assert.That(results?.Contains(4) ?? false);
            Assert.That(results?.Contains(5) ?? false);
            Assert.That(results?.Contains(6) ?? false);
        });
    }

    #endregion SetBestStoryIds tests

    #region GetStory tests

    [Test]
    public void GetStoryShouldReturnNullWhenRequestedStoryIsNotFoundInCache()
    {
        var result = _sut.GetStory(1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetStoryShouldReturnNullWhenCachingIsDisabled()
    {
        _configurationProvider.GetCacheRetentionSeconds().Returns("0");
        var story = new Story();
        _sut.AddOrUpdateStory(1, story);

        var result = _sut.GetStory(1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetStoryShouldReturnNullWhenStoryHasExpired()
    {
        _configurationProvider.GetCacheRetentionSeconds().Returns("1");
        var story = new Story();
        _sut.AddOrUpdateStory(1, story);
        Thread.Sleep(1001);

        var result = _sut.GetStory(1);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetStoryShouldReturnTheRelevantStoryWhenItIsFoundInCache()
    {
        var story = new Story();
        _sut.AddOrUpdateStory(1, story);

        var result = _sut.GetStory(1);

        Assert.That(result, Is.EqualTo(story));
    }

    #endregion GetStory tests

    #region AddOrUpdateStory tests

    [Test]
    public void AddOrUpdateStoryShouldUpdateTheCachedStory()
    {
        var story = new Story();
        var updatedStory = new Story();
        _sut.AddOrUpdateStory(1, story);

        _sut.AddOrUpdateStory(1, updatedStory);

        var result = _sut.GetStory(1);
        Assert.That(result, Is.EqualTo(updatedStory));
    }

    #endregion AddOrUpdateStory tests
}