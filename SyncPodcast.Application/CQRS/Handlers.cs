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

        public RegisterUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<AuthUserResultDTO> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            User? user = await _userRepository.GetByUsernameAsync(request.Username, ct);
            if (user != null)
            {
                throw new DomainException("Username is already taken.");
            }
            user = new User(Guid.NewGuid() ,request.Username, request.email, request.Password, DateTime.UtcNow);

            await _userRepository.AddAsync(user, ct);
            var Token = _tokenService.GenerateToken(user.ID);

            return new AuthUserResultDTO (
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
        public LoginUserCommandHandler(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }
        public async Task<AuthUserResultDTO> Handle(LoginUserCommand request, CancellationToken ct)
        {
            string hashedPassword = _hashService.Hash(request.Password);
            User? user = await _userRepository.GetByUsernameAsync(request.Username, ct);
            if (user == null || user.PasswordHash != hashedPassword)
            {
                throw new DomainException("Invalid username or password.");
            }
            var Token = _tokenService.GenerateToken(user.ID);
            return new AuthUserResultDTO(
                user.ID,
                user.Username,
                Token.AccessToken,
                Token.RefreshToken,
                Token.ExpiresAt
            );
        }
    }
    public class SubscribePodcastCommandHandler : IRequestHandler<SubscibePodcastCommand, SubscibeResultDTO>
    {
        private readonly IPoscastRepository _podcasts;
        private readonly ISubscriptionRepository _subscriptions;
        private readonly IRssParser _rssParser;

        public SubscribePodcastCommandHandler(IPoscastRepository podcasts, ISubscriptionRepository subscriptions, IRssParser rssParser)
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
}
