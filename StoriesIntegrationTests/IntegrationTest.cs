using System.Diagnostics;
using RestSharp;
using StoriesAPI.Models;

namespace StoriesIntegrationTests;

[TestFixture]
public class IntegrationTest
{
    private readonly RestClient _client = new(new RestClientOptions("https://localhost:7002/api/stories"));
    private Process? _hostProcess;

    [OneTimeSetUp]
    public void Setup()
    {
        _hostProcess = Process.Start(@"..\..\..\..\StoriesAPI\bin\Debug\net8.0\StoriesAPI.exe");
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _hostProcess?.Kill();
        _hostProcess?.Dispose();
    }

    [TestCase(1)]
    [TestCase(200)]
    public void RequestWithValidCountShouldReturnRequestedNumberOfResults(int count)
    {
        var request = new RestRequest($"/best?count={count}");

        var results = _client.Get<IEnumerable<Story>>(request);

        Assert.That(results?.Count(), Is.EqualTo(count));
    }

    [Test]
    public void RequestWithInvalidCountShouldReturnABadRequestError()
    {
        var request = new RestRequest("/best?count=0");

        Assert.Throws<HttpRequestException>(() => _client.Get(request), "Request failed with status code BadRequest");
    }
}