using Moq;
using FluentAssertions;
using Xunit;
using SyncPodcast.Application.CQRS;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Domain.Exceptions;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Tests.Helpers;

namespace SyncPodcast.Application.Tests.Handlers;

public class GetPodcastDetailsQueryHandlerTests
{
    private readonly Mock<IPodcastRepository> _podcastRepo = new();
    private readonly GetPodcastDetailsQueryHandler _handler;

    public GetPodcastDetailsQueryHandlerTests()
    {
        _handler = new GetPodcastDetailsQueryHandler(_podcastRepo.Object);
    }

    [Fact]
    public async Task Handle_PodcastExists_ReturnsDTO()
    {
        // Arrange
        var podcast = EntityFactory.CreatePodcast(title: "Detail Pod", author: "Detail Author");
        var episode = EntityFactory.CreateEpisode(podcastId: podcast.ID, title: "Ep 1");
        podcast.AddEpisode(episode);

        _podcastRepo.Setup(r => r.GetPodcastByIdAsync(podcast.ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(podcast);

        // Act
        var result = await _handler.Handle(new GetPodcastDetailsQuery(podcast.ID), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Detail Pod");
        result.Author.Should().Be("Detail Author");
        result.Episodes.Should().HaveCount(1);
        result.Episodes.First().Title.Should().Be("Ep 1");
    }

    [Fact]
    public async Task Handle_PodcastNotFound_ThrowsDomainException()
    {
        // Arrange
        var podcastId = Guid.NewGuid();
        _podcastRepo.Setup(r => r.GetPodcastByIdAsync(podcastId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Podcast?)null);

        // Act & Assert
        await _handler.Invoking(h => h.Handle(new GetPodcastDetailsQuery(podcastId), CancellationToken.None))
            .Should().ThrowAsync<DomainException>()
            .WithMessage($"Podcast with ID {podcastId} not found.");
    }
}
