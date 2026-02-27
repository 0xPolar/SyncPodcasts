using FluentAssertions;
using FluentValidation;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUsername_Fails()
    {
        var command = new RegisterUserCommand("", "test@example.com", "password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Username));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_Fails(string email)
    {
        var command = new RegisterUserCommand("testuser", email, "password123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "email");
    }

    [Fact]
    public void Validate_WithEmptyPassword_Fails()
    {
        var command = new RegisterUserCommand("testuser", "test@example.com", "");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
    }

    [Fact]
    public void Validate_WithShortPassword_Fails()
    {
        var command = new RegisterUserCommand("testuser", "test@example.com", "abc");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
    }
}
