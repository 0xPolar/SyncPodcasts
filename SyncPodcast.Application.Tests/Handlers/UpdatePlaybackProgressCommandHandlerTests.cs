using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class UpdatePlaybackProgressCommandHandlerTests
{
    private readonly Mock<IPlaybackProgressRepository> _progressRepo = new();
    private readonly Mock<IPodcastRepository> _podcastRepo = new();
    private readonly UpdatePlaybackProgressCommandHandler _handler;

    public UpdatePlaybackProgressCommandHandlerTests()
    {
        _handler = new UpdatePlaybackProgressCommandHandler(_progressRepo.Object, _podcastRepo.Object);
    }

    [Fact]
    public async Task Handle_NewProgress_CreatesRecord()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var episodeId = Guid.NewGuid();
        var episode = EntityFactory.CreateEpisode(podcastId: Guid.NewGuid(), duration: TimeSpan.FromMinutes(60));

        _podcastRepo.Setup(r => r.GetEpisodeByIdAsync(episodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);
        _progressRepo.Setup(r => r.GetAsync(userId, episodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PlaybackProgress?)null);

        var command = new UpdatePlaybackProgressCommand(userId, episodeId, TimeSpan.FromMinutes(10));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _progressRepo.Verify(r => r.SaveAsync(
            It.Is<PlaybackProgress>(p => p.UserID == userId && p.EpisodeID == episodeId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingProgress_UpdatesRecord()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var episodeId = Guid.NewGuid();
        var episode = EntityFactory.CreateEpisode(podcastId: Guid.NewGuid(), duration: TimeSpan.FromMinutes(60));
        var existingProgress = EntityFactory.CreatePlaybackProgress(userId: userId, episodeId: episodeId, position: TimeSpan.FromMinutes(5));

        _podcastRepo.Setup(r => r.GetEpisodeByIdAsync(episodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(episode);
        _progressRepo.Setup(r => r.GetAsync(userId, episodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProgress);

        var command = new UpdatePlaybackProgressCommand(userId, episodeId, TimeSpan.FromMinutes(30));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingProgress.Position.Should().Be(TimeSpan.FromMinutes(30));
        _progressRepo.Verify(r => r.SaveAsync(existingProgress, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EpisodeNotFound_ThrowsDomainException()
    {
        // Arrange
        var episodeId = Guid.NewGuid();
        _podcastRepo.Setup(r => r.GetEpisodeByIdAsync(episodeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Episode?)null);

        var command = new UpdatePlaybackProgressCommand(Guid.NewGuid(), episodeId, TimeSpan.FromMinutes(10));

        // Act & Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage($"Episode with ID {episodeId} not found.");
    }
}
