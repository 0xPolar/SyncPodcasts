using FluentAssertions;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "currentPass1", "newPassword123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var command = new ChangePasswordCommand(Guid.Empty, "currentPass1", "newPassword123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.UserId));
    }

    [Fact]
    public void Validate_WithEmptyCurrentPassword_Fails()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "", "newPassword123");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.CurrentPassword));
    }

    [Fact]
    public void Validate_WithEmptyNewPassword_Fails()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "currentPass1", "");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.NewPassword));
    }

    [Fact]
    public void Validate_WithShortNewPassword_Fails()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "currentPass1", "abc");
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.NewPassword));
    }
}
