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
        pricate readonly 
        public Task<AuthUserResultDTO> Handle(RegisterUserCommand request, CancellationToken ct)
        {
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
            var podcast = await _podcasts.GetByFeedUrlAsync(request.FeedURL, ct);

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
