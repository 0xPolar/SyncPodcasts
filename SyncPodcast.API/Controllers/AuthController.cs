using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyncPodcast.Application.CQRS;
using SyncPodcast.API.Extensions;

namespace SyncPodcast.API.Controllers;

public record ChangePasswordRequest(string CurentPassword, string NewPassword);

[ApiController]
[Route("api/v1/auth")]
public class  AuthController : ControllerBase
{
    private readonly ISender _mediator;

    public AuthController(ISender mediator)
    {
        _mediator = mediator;
    }

    // POST: api/v1/auth/register
    // BODY: { "username": "user1", "email":"uesr1@example.com", "password": "pass123" }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST: api/v1/auth/login
    // BODY: { "username": "user1", "password": "pass123" }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST: api/v1/auth/refresh
    // Body: { "accessToken": "eyJhbG...", "refreshToken": "abc123..." }
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // POST: api/v1/auth/logout
    // No Body, requires Authorization header with Bearer token
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.GetUserId();
        await _mediator.Send(new RevokeTokenCommand(userId));
        return NoContent();
    }

    //PUT: api/v1/auth/password
    // BODY: { "currentPassword": "pass123", "newPassword": "new" }
    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = User.GetUserId();
        await _mediator.Send(new ChangePasswordCommand(userId, request.CurentPassword, request.NewPassword));
        return NoContent();
    }

}
