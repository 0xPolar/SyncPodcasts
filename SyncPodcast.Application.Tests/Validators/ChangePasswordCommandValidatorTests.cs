using FluentValidation.TestHelper;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "currentPass1", "newPassword123");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var command = new ChangePasswordCommand(Guid.Empty, "currentPass1", "newPassword123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WithEmptyCurrentPassword_Fails()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "", "newPassword123");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    [Fact]
    public void Validate_WithEmptyNewPassword_Fails()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "currentPass1", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    [Fact]
    public void Validate_WithShortNewPassword_Fails()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "currentPass1", "abc");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }
}
