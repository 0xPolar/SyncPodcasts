using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;

namespace SyncPodcast.Application.Tests.Handlers;

public class UnsubscribePodcastCommandHandlerTests
{
    private readonly Mock<IPodcastRepository> _podcastRepo = new();
    private readonly Mock<ISubscriptionRepository> _subRepo = new();
    private readonly UnsubscribePodcastCommandHandler _handler;

    public UnsubscribePodcastCommandHandlerTests()
    {
        _handler = new UnsubscribePodcastCommandHandler(_podcastRepo.Object, _subRepo.Object);
    }

    [Fact]
    public async Task Handle_SubscribedUser_Unsubscribes()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var podcastId = Guid.NewGuid();
        _subRepo.Setup(r => r.ExistsAsync(userId, podcastId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UnsubscribePodcastCommand(userId, podcastId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _subRepo.Verify(r => r.DeleteAsync(userId, podcastId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NotSubscribed_ThrowsDomainException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var podcastId = Guid.NewGuid();
        _subRepo.Setup(r => r.ExistsAsync(userId, podcastId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UnsubscribePodcastCommand(userId, podcastId);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage("User is not subscribed to this podcast.");
    }
}
