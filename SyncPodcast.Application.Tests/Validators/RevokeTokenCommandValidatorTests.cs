using FluentAssertions;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class RevokeTokenCommandValidatorTests
{
    private readonly RevokeTokenCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new RevokeTokenCommand(Guid.NewGuid());
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var command = new RevokeTokenCommand(Guid.Empty);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.UserID));
    }
}
