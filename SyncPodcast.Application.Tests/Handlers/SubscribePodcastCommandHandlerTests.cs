using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class SubscribePodcastCommandHandlerTests
{
    private readonly Mock<IPodcastRepository> _podcastRepo = new();
    private readonly Mock<ISubscriptionRepository> _subRepo = new();
    private readonly Mock<IRssParser> _rssParser = new();
    private readonly SubscribePodcastCommandHandler _handler;

    public SubscribePodcastCommandHandlerTests()
    {
        _handler = new SubscribePodcastCommandHandler(_podcastRepo.Object, _subRepo.Object, _rssParser.Object);
    }

    [Fact]
    public async Task Handle_NewPodcast_ParsesFeedAndCreatesSubscription()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedUrl = new Uri("https://example.com/feed.xml");
        var podcast = EntityFactory.CreatePodcast(title: "New Pod", feedUrl: feedUrl.ToString());

        _podcastRepo.Setup(r => r.GetByFeedUrlAsync(feedUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Podcast?)null);
        _rssParser.Setup(r => r.ParseAsync(feedUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(podcast);
        _subRepo.Setup(r => r.ExistsAsync(userId, podcast.ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new SubscribePodcastCommand(userId, feedUrl);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Pod");
        result.PodcastId.Should().Be(podcast.ID);
        _podcastRepo.Verify(r => r.AddAsync(podcast, It.IsAny<CancellationToken>()), Times.Once);
        _subRepo.Verify(r => r.AddAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AlreadySubscribed_ThrowsDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var feedUrl = new Uri("https://example.com/feed.xml");
        var podcast = EntityFactory.CreatePodcast(feedUrl: feedUrl.ToString());

        _podcastRepo.Setup(r => r.GetByFeedUrlAsync(feedUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(podcast);
        _subRepo.Setup(r => r.ExistsAsync(userId, podcast.ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new SubscribePodcastCommand(userId, feedUrl);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("User is already subscribed to this podcast.");
    }
}
