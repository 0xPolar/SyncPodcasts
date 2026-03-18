using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class GetPlaybackProgressQueryHandlerTests
{
    private readonly Mock<IPlaybackProgressRepository> _progressRepo = new();
    private readonly GetPlaybackProgressQueryHandler _handler;

    public GetPlaybackProgressQueryHandlerTests()
    {
        _handler = new GetPlaybackProgressQueryHandler(_progressRepo.Object);
    }

    [Fact]
    public async Task Handle_NoProgress_ReturnsZero()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var episodeId = Guid.NewGuid();
        _progressRepo.Setup(r => r.GetAsync(userId, episodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlaybackProgress?)null);

        // Act
        var result = await _handler.Handle(new GetPlaybackProgressQuery(userId, episodeId), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.EpisodeId.Should().Be(episodeId);
        result.Progress.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public async Task Handle_ExistingProgress_ReturnsPosition()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var episodeId = Guid.NewGuid();
        var progress = EntityFactory.CreatePlaybackProgress(userId: userId, episodeId: episodeId, position: TimeSpan.FromMinutes(15));
        _progressRepo.Setup(r => r.GetAsync(userId, episodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progress);

        // Act
        var result = await _handler.Handle(new GetPlaybackProgressQuery(userId, episodeId), CancellationToken.None);

        // Assert
        result.EpisodeId.Should().Be(episodeId);
        result.Progress.Should().Be(TimeSpan.FromMinutes(15));
    }
}
