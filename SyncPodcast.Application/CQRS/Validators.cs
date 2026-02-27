using System;
using MediatR;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace SyncPodcast.Application.CQRS
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }

    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.AccessToken).NotEmpty().WithMessage("Access token is required.");
            RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required.");
        }
    }

    public class RevokeTokenCommandValidator : AbstractValidator<RevokeTokenCommand>
    {
        public RevokeTokenCommandValidator()
        {
            RuleFor(x => x.UserID).NotEmpty().WithMessage("User ID is required.");
        }
    }

    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.CurrentPassword).NotEmpty().WithMessage("Current password is required.");
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(6).WithMessage("New password must be at least 6 characters long.");
        }
    }
    public class SubscibePodcastCommandValidator : AbstractValidator<SubscribePodcastCommand>
    {
        public SubscibePodcastCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.FeedURL).NotEmpty().Must(uri => Uri.IsWellFormedUriString(uri.ToString(), UriKind.Absolute)).WithMessage("Valid feed URL is required.");
        }
    }

    public class UnsubscribePodcastCommandValidator : AbstractValidator<UnsubscribePodcastCommand>
    {
        public UnsubscribePodcastCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.PodcastId).NotEmpty().WithMessage("Podcast ID is required.");
        }
    }

    public class UpdatePlaybackProgressCommandValidator : AbstractValidator<UpdatePlaybackProgressCommand>
    {
        public UpdatePlaybackProgressCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.EpisodeId).NotEmpty().WithMessage("Episode ID is required.");
            RuleFor(x => x.Progress).GreaterThanOrEqualTo(TimeSpan.Zero).WithMessage("Progress must be a positive time span.");
        }
    }

    public class GetLibraryQueryValidator : AbstractValidator<GetLibraryQuery>
    {
        public GetLibraryQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }

    public class SearchPodcastQueryValidator : AbstractValidator<SearchPodcastQuery>
    {
        public SearchPodcastQueryValidator()
        {
            RuleFor(x => x.Query).NotEmpty().WithMessage("Search query is required.");
            RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.");
        }
    }

    public class GetPodcastDetailsQueryValidator : AbstractValidator<GetPodcastDetailsQuery>
    {
        public GetPodcastDetailsQueryValidator()
        {
            RuleFor(x => x.PodcastId).NotEmpty().WithMessage("Podcast ID is required.");
        }
    }

    public class GetPlaybackProgressQueryValidator : AbstractValidator<GetPlaybackProgressQuery>
    {
        public GetPlaybackProgressQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.EpisodeId).NotEmpty().WithMessage("Episode ID is required.");
        }
    }

}
