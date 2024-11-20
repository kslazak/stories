using NUnit.Framework;
using RestSharp;
using StoriesAPI.Models;
using TechTalk.SpecFlow.Assist;

namespace StoriesBehaviorTests.StepDefinitions;

[Binding]
public sealed class StorySteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private readonly RestClient _client = new(new RestClientOptions("https://localhost:7002/api/stories"));

    [When("GetBestStories is called with count parameter equal to '(.*)'")]
    public void WhenGetBestStoriesIsCalledWithCountParameterEqualTo(string count)
    {
        var request = new RestRequest($"/best?count={count}");
        var results = _client.Get<IEnumerable<Story>>(request);
        _scenarioContext.Set(results, "results");
    }

    [When("GetBestStories is called with invalid count parameter equal to '(.*)'")]
    public void WhenGetBestStoriesIsCalledWithInvalidCountParameterEqualTo(string count)
    {
        var request = new RestRequest($"/best?count={count}");
        try
        {
            _client.Get(request);
        }
        catch (Exception exception)
        {
            _scenarioContext.Set(exception, "exception");
        }
    }

    [When("GetBestStories is called without count parameter")]
    public void WhenGetBestStoriesIsCalledWithoutCountParameter()
    {
        var request = new RestRequest("/best");
        try
        {
            _client.Get(request);
        }
        catch (Exception exception)
        {
            _scenarioContext.Set(exception, "exception");
        }
    }

    [Then("the following results are being returned")]
    public void ThenTheFollowingResultsAreBeingReturned(Table table)
    {
        var actualResults = _scenarioContext.Get<IEnumerable<Story>>("results");
        table.CompareToSet(actualResults);
    }

    [Then("(.*) results are being returned")]
    public void ThenResultsAreBeingReturned(int resultCount)
    {
        var actualResults = _scenarioContext.Get<IEnumerable<Story>>("results");
        Assert.That(actualResults, Has.Count.EqualTo(resultCount));
    }

    [Then("the '(.*)' error is being returned")]
    public void ThenTheErrorIsBeingReturned(string errorMessage)
    {
        var exception = _scenarioContext.Get<Exception>("exception");
        Assert.That(exception.Message, Is.EqualTo(errorMessage));
    }
}
