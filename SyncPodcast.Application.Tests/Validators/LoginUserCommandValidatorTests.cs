using FluentAssertions;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class LoginUserCommandValidatorTests
{
    private readonly LoginUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new LoginUserCommand("testuser", "password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUsername_Fails()
    {
        var command = new LoginUserCommand("", "password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Username));
    }

    [Fact]
    public void Validate_WithEmptyPassword_Fails()
    {
        var command = new LoginUserCommand("testuser", "");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
    }
}
