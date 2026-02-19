using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncPodcast.Application.CQRS;
using SyncPodcast.API.Extensions;

namespace SyncPodcast.API.Controllers;

[ApiController]
[Route("api/v1/library")]
[Authorize]
public class LibraryController : ControllerBase
{
    private readonly ISender _mediator;
    public LibraryController(ISender mediator)
    {
        _mediator = mediator;
    }
    // GET: api/v1/library
    [HttpGet("api/v1/library")]
    public async Task<IActionResult> GetLibrary()
    {
        var userId = User.GetUserId();
        var result = await _mediator.Send(new GetLibraryQuery(userId));
        return Ok(result);
    }
}
