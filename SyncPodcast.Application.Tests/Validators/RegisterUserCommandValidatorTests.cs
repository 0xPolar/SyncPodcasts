using FluentValidation.TestHelper;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class RegisterUserCommandValidatorTests
{
    private readonly RegisterUserCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new RegisterUserCommand("testuser", "test@example.com", "password123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUsername_Fails()
    {
        var command = new RegisterUserCommand("", "test@example.com", "password123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Validate_WithInvalidEmail_Fails(string email)
    {
        var command = new RegisterUserCommand("testuser", email, "password123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.email);
    }

    [Fact]
    public void Validate_WithEmptyPassword_Fails()
    {
        var command = new RegisterUserCommand("testuser", "test@example.com", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_WithShortPassword_Fails()
    {
        var command = new RegisterUserCommand("testuser", "test@example.com", "abc");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}
