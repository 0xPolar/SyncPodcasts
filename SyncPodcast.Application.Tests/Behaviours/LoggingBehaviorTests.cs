using Moq;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Xunit;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Behaviours;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_ReturnsResultFromNextUnchanged()
    {
        // Arrange
        var logger = new Mock<ILogger<LoggingBehavior<LoginUserCommand, AuthUserResultDTO>>>();
        var behavior = new LoggingBehavior<LoginUserCommand, AuthUserResultDTO>(logger.Object);

        var expected = new AuthUserResultDTO(Guid.NewGuid(), "user", "token", "refresh", DateTime.UtcNow);
        var next = new Mock<RequestHandlerDelegate<AuthUserResultDTO>>();
        next.Setup(n => n()).ReturnsAsync(expected);

        // Act
        var result = await behavior.Handle(new LoginUserCommand("user", "pass"), next.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
        next.Verify(n => n(), Times.Once);
    }
}
