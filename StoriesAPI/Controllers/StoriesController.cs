using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using StoriesAPI.Models;

namespace StoriesAPI.Controllers;

/// <summary>
/// Handles requests for the Stories.
/// </summary>
[ApiController]
[Route("api/stories")]
public class StoriesController(ILogger<StoriesController> logger, IStoryService storyService) : ControllerBase
{
    private readonly ILogger<StoriesController> _logger = logger;
    private readonly IStoryService _storyService = storyService;

    /// <summary>
    /// Gets the specified number of best stories.
    /// </summary>
    /// <param name="count">Number of stories to retrieve (between 1 and 200).</param>
    /// <returns>Retrieved stories.</returns>
    /// <response code="200">Returns the specified number of stories.</response>
    /// <response code="400">If the <b>count</b> parameter is invalid.</response>
    [HttpGet("best")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public ActionResult<IEnumerable<Story>> GetBestStories([Required, Range(1, 200)] int count)
    {
        try
        {
            _logger.LogAsInformation($"GetBestStories called with count = {count}.", HttpContext);

            var ids = _storyService.GetBestStoryIds(count);
            return Ok(_storyService.GetStories(ids));
        }
        catch (Exception exception)
        {
            _logger.LogAsError(exception.Message, HttpContext);
            return Problem("Internal error occurred.", null, 500);
        }
        finally
        {
            _logger.LogAsInformation("GetBestStories completed.", HttpContext);
        }
    }
}