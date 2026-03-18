using Moq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Xunit;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Behaviours;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_NoValidators_CallsNext()
    {
        // Arrange
        var behavior = new ValidationBehavior<LoginUserCommand, AuthUserResultDTO>(
            Enumerable.Empty<IValidator<LoginUserCommand>>());

        var expected = new AuthUserResultDTO(Guid.NewGuid(), "user", "token", "refresh", DateTime.UtcNow);
        var next = new Mock<RequestHandlerDelegate<AuthUserResultDTO>>();
        next.Setup(n => n()).ReturnsAsync(expected);

        // Act
        var result = await behavior.Handle(new LoginUserCommand("user", "pass"), next.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
        next.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationPasses_CallsNext()
    {
        // Arrange
        var validator = new Mock<IValidator<LoginUserCommand>>();
        validator.Setup(v => v.Validate(It.IsAny<ValidationContext<LoginUserCommand>>()))
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<LoginUserCommand, AuthUserResultDTO>(
            new[] { validator.Object });

        var expected = new AuthUserResultDTO(Guid.NewGuid(), "user", "token", "refresh", DateTime.UtcNow);
        var next = new Mock<RequestHandlerDelegate<AuthUserResultDTO>>();
        next.Setup(n => n()).ReturnsAsync(expected);

        // Act
        var result = await behavior.Handle(new LoginUserCommand("user", "pass"), next.Object, CancellationToken.None);

        // Assert
        result.Should().Be(expected);
        next.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidationFails_ThrowsValidationException()
    {
        // Arrange
        var validator = new Mock<IValidator<LoginUserCommand>>();
        validator.Setup(v => v.Validate(It.IsAny<ValidationContext<LoginUserCommand>>()))
            .Returns(new ValidationResult(new[] { new ValidationFailure("Username", "Required") }));

        var behavior = new ValidationBehavior<LoginUserCommand, AuthUserResultDTO>(
            new[] { validator.Object });

        var next = new Mock<RequestHandlerDelegate<AuthUserResultDTO>>();

        // Act & Assert
        await behavior.Invoking(b => b.Handle(
                new LoginUserCommand("", "pass"), next.Object, CancellationToken.None))
            .Should().ThrowAsync<ValidationException>();

        next.Verify(n => n(), Times.Never);
    }
}
