using MediatR;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using SyncPodcast.Domain.Entities;

namespace SyncPodcast.Application.CQRS
{

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthUserResultDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IHashService _hashService;

        public RegisterUserCommandHandler(IUserRepository userRepository, ITokenService tokenService, IHashService hashService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _hashService = hashService;
        }
        public async Task<AuthUserResultDTO> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            User? user = await _userRepository.GetByUsernameAsync(request.Username, ct);
            if (user != null)
            {
                throw new DomainException("Username is already taken.");
            }

            string hashedPassword = _hashService.Hash(request.Password);
            user = new User(Guid.NewGuid(), request.Username, request.email, hashedPassword, DateTime.UtcNow);

            await _userRepository.AddAsync(user, ct);
            var Token = _tokenService.GenerateToken(user.ID);

            user.SetRefreshToken(Token.RefreshToken, Token.RefreshTokenExpiresAt);
            await _userRepository.UpdateUserAsync(user, ct);

            return new AuthUserResultDTO(
                user.ID,
                user.Username,
                Token.AccessToken,
                Token.RefreshToken,
                Token.ExpiresAt
            );
        }
    }

    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthUserResultDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IHashService _hashService;
        public LoginUserCommandHandler(IUserRepository userRepository, ITokenService tokenService, IHashService hashService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _hashService = hashService;
        }
        public async Task<AuthUserResultDTO> Handle(LoginUserCommand request, CancellationToken ct)
        {
            User? user = await _userRepository.GetByUsernameAsync(request.Username, ct);
            if (user == null || !_hashService.Verify(request.Password, user.PasswordHash) )
            {
                throw new DomainException("Invalid username or password.");
            }
            var Token = _tokenService.GenerateToken(user.ID);

            user.SetRefreshToken(Token.RefreshToken, Token.RefreshTokenExpiresAt);
            await _userRepository.UpdateUserAsync(user, ct);

            return new AuthUserResultDTO(
                user.ID,
                user.Username,
                Token.AccessToken,
                Token.RefreshToken,
                Token.ExpiresAt
            );
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthUserResultDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        public RefreshTokenCommandHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<AuthUserResultDTO> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var result = _tokenService.RefreshToken(request.AccessToken)
                ?? throw new DomainException("Invalid access token.");

            var user = await _userRepository.GetByIdAsync(result.userID, ct)
                ?? throw new NotFoundException("User", result.userID);

            user.ValidateRefreshToken(request.RefreshToken);

            user.SetRefreshToken(result.token.RefreshToken, result.token.RefreshTokenExpiresAt);
            await _userRepository.UpdateUserAsync(user, ct);

            return new AuthUserResultDTO(
                user.ID,
                user.Username,
                result.token.AccessToken,
                result.token.RefreshToken,
                result.token.ExpiresAt
            );
        }
    }

    public class  RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenSerivce;

        public RevokeTokenCommandHandler(IUserRepository userRepository, ITokenService tokenSerivce)
        {
            _userRepository = userRepository;
            _tokenSerivce = tokenSerivce;
        }

        public async Task Handle(RevokeTokenCommand request, CancellationToken ct) 
        {
            User? user = await _userRepository.GetByIdAsync(request.UserID, ct)
                ?? throw new NotFoundException("User", request.UserID);

            user.ClearRefreshToken();
            await _userRepository.UpdateUserAsync(user, ct);
        }
        
    }
    public class SubscribePodcastCommandHandler : IRequestHandler<SubscibePodcastCommand, SubscibeResultDTO>
    {
        private readonly IPodcastRepository _podcasts;
        private readonly ISubscriptionRepository _subscriptions;
        private readonly IRssParser _rssParser;

        public SubscribePodcastCommandHandler(IPodcastRepository podcasts, ISubscriptionRepository subscriptions, IRssParser rssParser)
        {
            _podcasts = podcasts;
            _subscriptions = subscriptions;
            _rssParser = rssParser;
        }

        public async Task<SubscibeResultDTO> Handle(SubscibePodcastCommand request, CancellationToken ct)
        {
            Podcast? podcast = await _podcasts.GetByFeedUrlAsync(request.FeedURL, ct);

            if (podcast == null)
            {
                podcast = await _rssParser.ParseAsync(request.FeedURL, ct);
                await _podcasts.AddAsync(podcast, ct);
            }

            if (await _subscriptions.ExistsAsync(request.UserId, podcast.ID, ct))
            {
                throw new DomainException("User is already subscribed to this podcast.");
            }

            var subscription = new Subscription(request.UserId, podcast.ID);
            await _subscriptions.AddAsync(subscription, ct);

            return new SubscibeResultDTO(
                podcast.ID,
                podcast.Title,
                podcast.Author,
                podcast.FeedUrl,
                podcast.ArtworkUrl
            );
        }
    }

    public class UnsubscribePodcastCommandHandler : IRequestHandler<UnsubscribePodcastCommand>
    {
        private readonly IPodcastRepository _podcasts;
        private readonly ISubscriptionRepository _subscriptions;
        public UnsubscribePodcastCommandHandler(IPodcastRepository podcasts, ISubscriptionRepository subscriptions)
        {
            _subscriptions = subscriptions;
            _podcasts = podcasts;
        }
        public async Task Handle(UnsubscribePodcastCommand request, CancellationToken ct)
        {
            if (!await _subscriptions.ExistsAsync(request.UserId, request.PodcastId, ct))
            {
                throw new DomainException("User is not subscribed to this podcast.");
            }
            await _subscriptions.DeleteAsync(request.UserId, request.PodcastId, ct);
        }

    }

    public class UpdatePlaybackProgressCommandHandler : IRequestHandler<UpdatePlaybackProgressCommand>
    {
        private readonly IPlaybackProgressRepository _playbackProgressRepository;
        private readonly IPodcastRepository _podcastRepository;

        public UpdatePlaybackProgressCommandHandler(IPlaybackProgressRepository playbackProgressRepository, IPodcastRepository podcastRepository)
        {
            _playbackProgressRepository = playbackProgressRepository;
            _podcastRepository = podcastRepository;
        }

        public async Task Handle(UpdatePlaybackProgressCommand request, CancellationToken ct)
        {
            var episode = await _podcastRepository.GetEpisodeByIdAsync(request.EpisodeId, ct);
            if (episode == null)
                throw new DomainException($"Episode with ID {request.EpisodeId} not found.");

            var progress = await _playbackProgressRepository.GetAsync(request.UserId, request.EpisodeId, ct);
            if (progress == null)
            {
                progress = new PlaybackProgress(request.UserId, request.EpisodeId, request.Progress, false);
            }

            progress.UpdatePosition(request.Progress, episode.Duration);
            await _playbackProgressRepository.SaveAsync(progress, ct);
        }
    }
}

