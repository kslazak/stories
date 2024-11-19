using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using StoriesAPI;
using StoriesAPI.Models;

namespace StoriesUnitTests;

[TestFixture]
public class StoryServiceTest
{
    private ILogger<StoryService> _logger;
    private IStoryProvider _storyProvider;
    private IStoryCache _storyCache;
    private IHttpContextAccessor _httpContextAccessor;
    private StoryService _sut;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger<StoryService>>();
        _storyProvider = Substitute.For<IStoryProvider>();
        _storyCache = Substitute.For<IStoryCache>();
        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

        _sut = new StoryService(_logger, _storyProvider, _storyCache, _httpContextAccessor);
    }

    #region GetBestStoryIds tests

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void WhenStoryIdsAreFoundInCacheGetBestStoryIdsShouldReturnTheseIdsAndShouldNotInvokeStoryProvider(int count)
    {
        _storyCache.GetBestStoryIds(count).Returns(TestUtils.CreateListOfIds(count));

        var results = _sut.GetBestStoryIds(count);

        Assert.That(results.Count == count);
        _storyProvider.DidNotReceive().GetBestStoryIds();
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void WhenStoryIdsAreFoundInCacheGetBestStoryIdsShouldNotUpdateTheCache(int count)
    {
        _storyCache.GetBestStoryIds(count).Returns(TestUtils.CreateListOfIds(count));

        _sut.GetBestStoryIds(count);

        _storyCache.DidNotReceive().SetBestStoryIds(Arg.Any<IEnumerable<int>>());
    }

    [TestCase(1, 200)]
    [TestCase(100, 200)]
    [TestCase(200, 200)]
    [TestCase(200, 190)]
    public void WhenStoryIdsAreNotFoundInCacheGetBestStoryIdsShouldInvokeStoryProviderAndReturnTheRelevantNumberOfIds(int requestedCount, int providedCount)
    {
        _storyCache.GetBestStoryIds(requestedCount).Returns(default(List<int>));
        _storyProvider.GetBestStoryIds().Returns(Task.FromResult(TestUtils.CreateListOfIds(providedCount)));

        var results = _sut.GetBestStoryIds(requestedCount);

        Assert.That(results.Count == Math.Min(requestedCount, providedCount));
    }

    [TestCase(1, 200)]
    [TestCase(100, 200)]
    [TestCase(200, 200)]
    [TestCase(200, 190)]
    public void WhenStoryIdsAreNotFoundInCacheGetBestStoryIdsShouldInvokeStoryProviderAndUpdateTheCache(int requestedCount, int providedCount)
    {
        var ids = TestUtils.CreateListOfIds(providedCount);
        _storyCache.GetBestStoryIds(requestedCount).Returns(default(List<int>));
        _storyProvider.GetBestStoryIds().Returns(Task.FromResult(ids));

        _sut.GetBestStoryIds(requestedCount);

        _storyCache.Received(1).SetBestStoryIds(ids);
    }

    #endregion GetBestStoryIds tests

    #region GetStories tests

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void WhenAllRequestedStoriesAreFoundInCacheGetStoriesShouldReturnTheseStoriesAndShouldNotInvokeStoryProvider(int count)
    {
        var ids = TestUtils.CreateListOfIds(count);
        foreach (var id in ids)
            _storyCache.GetStory(id).Returns(TestUtils.CreateStory());

        var results = _sut.GetStories(ids);

        Assert.That(results.Count() == count);
        _storyProvider.DidNotReceive().GetStory(Arg.Any<int>());
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void WhenAllRequestedStoriesAreFoundInCacheGetStoriesShouldNotUpdateStoryCache(int count)
    {
        var ids = TestUtils.CreateListOfIds(count);
        foreach (var id in ids)
            _storyCache.GetStory(id).Returns(TestUtils.CreateStory());

        _sut.GetStories(ids);

        _storyCache.DidNotReceive().AddOrUpdateStory(Arg.Any<int>(), Arg.Any<Story>());
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void WhenNoStoriesAreFoundInCacheGetStoriesShouldInvokeStoryProviderAndReturnTheStories(int count)
    {
        var storyRetrievalTask = new Task<Story?>(TestUtils.CreateStory);
        storyRetrievalTask.RunSynchronously();
        var ids = TestUtils.CreateListOfIds(count);
        foreach (var id in ids)
        {
            _storyCache.GetStory(id).Returns(default(Story));
            _storyProvider.GetStory(id).Returns(storyRetrievalTask);
        }
        
        var results = _sut.GetStories(ids);

        Assert.That(results.Count() == count);
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(200)]
    public void WhenNoStoriesAreFoundInCacheGetStoriesShouldInvokeStoryProviderAndUpdateStoryCache(int count)
    {
        var storyRetrievalTask = new Task<Story?>(TestUtils.CreateStory);
        storyRetrievalTask.RunSynchronously();
        var ids = TestUtils.CreateListOfIds(count);
        foreach (var id in ids)
        {
            _storyCache.GetStory(id).Returns(default(Story));
            _storyProvider.GetStory(id).Returns(storyRetrievalTask);
        }

        _sut.GetStories(ids);

        foreach (var id in ids)
            _storyCache.Received(1).AddOrUpdateStory(id, Arg.Any<Story>());
    }

    [Test]
    public void WhenSomeStoriesAreNotFoundInCacheGetStoriesShouldInvokeStoryProviderForMissingStoriesOnlyAndReturnAllStories()
    {
        var storyRetrievalTask = new Task<Story?>(TestUtils.CreateStory);
        storyRetrievalTask.RunSynchronously();
        var ids = TestUtils.CreateListOfIds(2);
        _storyCache.GetStory(0).Returns(default(Story));
        _storyCache.GetStory(1).Returns(TestUtils.CreateStory());
        _storyProvider.GetStory(0).Returns(storyRetrievalTask);

        var results = _sut.GetStories(ids);

        Assert.That(results.Count() == 2);
        _storyProvider.DidNotReceive().GetStory(1);
        _storyCache.Received(1).AddOrUpdateStory(0, Arg.Any<Story>());
    }

    #endregion GetStories tests
}