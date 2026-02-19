using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncPodcast.Application.CQRS;
using SyncPodcast.API.Extensions;

namespace SyncPodcast.API.Controllers;

public record UpdatePlaybackProgressRequest(TimeSpan Progress);

[ApiController]
[Route("api/v1/playback")]
[Authorize]
public class PlaybackController : ControllerBase
{
    private readonly ISender _mediator;
    public PlaybackController(ISender mediator)
    {
        _mediator = mediator;
    }

    // GET: api/v1/playback/{episodeId}
    [HttpGet("{episodeId:guid}")]
    public async Task<IActionResult> GetPlaybackProgress(Guid episodeId)
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetPlaybackProgressQuery(userId, episodeId));
        return Ok(result);
    }

    // PUT: api/v1/playback/{episodeId}
    // Body: { "progress": "00:15:30" }
    [HttpPut("{episodeId:guid}")]
    public async Task<IActionResult> UpdatePlaybackProgress(Guid episodeId, UpdatePlaybackProgressRequest request)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new UpdatePlaybackProgressCommand(userId, episodeId, request.Progress));
        return NoContent();
    }
}
