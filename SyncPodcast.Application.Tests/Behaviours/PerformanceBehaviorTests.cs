using Moq;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Xunit;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Behaviours;

public class PerformanceBehaviorTests
{
    [Fact]
    public async Task Handle_FastRequest_DoesNotLogWarning()
    {
        // Arrange
        var logger = new Mock<ILogger<PerformanceBehavior<LoginUserCommand, AuthUserResultDTO>>>();
        var behavior = new PerformanceBehavior<LoginUserCommand, AuthUserResultDTO>(logger.Object);

        var expected = new AuthUserResultDTO(Guid.NewGuid(), "user", "token", "refresh", DateTime.UtcNow);
        var next = new Mock<RequestHandlerDelegate<AuthUserResultDTO>>();
        next.Setup(n => n()).ReturnsAsync(expected);

        // Act
        var result = await behavior.Handle(new LoginUserCommand("user", "pass"), next.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}
