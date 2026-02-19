using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncPodcast.Application.CQRS;
using SyncPodcast.API.Extensions;

namespace SyncPodcast.API.Controllers;


[ApiController]
[Route("api/v1/podcasts")]
[Authorize]

public record SubscribeRequest(Uri FeedURL);
public class  PodcastController : ControllerBase
{
    private readonly ISender _mediator;

    public PodcastController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/v1/podcasts/search?query=technology&page=1&pageSize=10
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new SearchPodcastQuery(query, page, pageSize));
        return Ok(result);
    }

    // GET: api/v1/podcasts/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPodcast(Guid id)
    {
        var result = await _mediator.Send(new GetPodcastDetailsQuery(id));
        return Ok(result);
    }

    // POST: api/v1/podcasts/subscribe
    // Body: { "feedURL": "http://example.com/podcast/feed.xml" }
    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] SubscribeRequest request)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new SubscibePodcastCommand(userId, request.FeedURL));
        return Ok(result);
    }

    // DELETE: api/v1/podcasts/{podcastId}/unsubscribe
    [HttpDelete("{podcastId:guid}/unsubscribe")]
    public async Task<IActionResult> Unsubscribe(Guid podcastId)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new UnsubscribePodcastCommand(userId, podcastId));
        return NoContent();
    }

}
