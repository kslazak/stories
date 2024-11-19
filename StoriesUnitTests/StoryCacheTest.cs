using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StoriesAPI;

namespace StoriesUnitTests;

[TestFixture]
public class StoryCacheTest
{
    private ILogger<StoryCache> _logger;
    private IConfiguration _configuration;
    private IHttpContextAccessor _httpContextAccessor;
    private StoryCache _sut;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger<StoryCache>>();
        _configuration = Substitute.For<IConfiguration>();
        _httpContextAccessor = Substitute.For<HttpContextAccessor>();

        _sut = new StoryCache(_logger, _configuration, _httpContextAccessor);
    }

    #region GetBestStoryIds tests

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void GetBestStoryIdsShouldReturnNullWhenNoIdsAreCached(int count)
    {
        var results = _sut.GetBestStoryIds(count);

        Assert.That(results == null);
    }

    [TestCase(2)]
    [TestCase(100)]
    [TestCase(200)]
    public void GetBestStoryIdsShouldReturnNullWhenLessIdsAreCachedThanRequested(int count)
    {
        _sut.SetBestStoryIds([1]);

        var results = _sut.GetBestStoryIds(count);

        Assert.That(results == null);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void GetBestStoryIdsShouldReturnRequestedNumberOfIdsWhenNumberOfCachedIdsIsGreaterOrEqualToRequestedCount(int count)
    {
        _sut.SetBestStoryIds([1, 2, 3]);

        var results = _sut.GetBestStoryIds(count);

        Assert.That(results?.Count == count);
    }

    #endregion GetBestStoryIds tests

    #region SetBestStoryIds tests

    [Test]
    public void SetBestStoryIdsShouldUpdateTheCachedIdsWithProvidedValues()
    {
        _sut.SetBestStoryIds([1, 2, 3]);
        _sut.SetBestStoryIds([4, 5, 6]);

        var results = _sut.GetBestStoryIds(3);

        Assert.That(results != null);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        Assert.That(results.Count == 3);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        Assert.That(results.Contains(4) && results.Contains(5) && results.Contains(6));
    }

    #endregion SetBestStoryIds tests

    #region GetStory tests

    [Test]
    public void GetStoryShouldReturnNullWhenRequestedStoryIsNotFoundInCache()
    {
        var result = _sut.GetStory(1);

        Assert.That(result == null);
    }

    [Test]
    public void GetStoryShouldReturnTheRelevantStoryWhenItIsFoundInCache()
    {
        var story = TestUtils.CreateStory();
        _sut.AddOrUpdateStory(1, story);

        var result = _sut.GetStory(1);

        Assert.That(result == story);
    }

    #endregion GetStory tests

    #region AddOrUpdateStory tests

    [Test]
    public void AddOrUpdateStoryShouldUpdateTheCachedStory()
    {
        var story = TestUtils.CreateStory();
        var updatedStory = TestUtils.CreateStory();
        _sut.AddOrUpdateStory(1, story);

        _sut.AddOrUpdateStory(1, updatedStory);

        var result = _sut.GetStory(1);
        Assert.That(result == updatedStory);
    }

    #endregion AddOrUpdateStory tests
}