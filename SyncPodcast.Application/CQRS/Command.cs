using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SyncPodcast.Application.CQRS
{
    // Auth Commands
    public record RegisterUserCommand(string Username, string email, string Password) : IRequest<AuthUserResultDTO>;
    public record LoginUserCommand(string Username, string Password) : IRequest<AuthUserResultDTO>;

    // Token Commands
    public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthUserResultDTO>;
    public record RevokeTokenCommand(Guid UserID) : IRequest;

    public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest;

    // Podcast Commands
    public record SubscibePodcastCommand(Guid UserId, Uri FeedURL) : IRequest<SubscibeResultDTO>;
    public record UnsubscribePodcastCommand(Guid UserId, Guid PodcastId) : IRequest;

    // Playback Commands
    public record UpdatePlaybackProgressCommand(Guid UserId, Guid EpisodeId, TimeSpan Progress) : IRequest;
}
