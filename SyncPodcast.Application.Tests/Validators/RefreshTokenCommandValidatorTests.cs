using FluentValidation.TestHelper;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new RefreshTokenCommand("some-access-token", "some-refresh-token");
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyAccessToken_Fails()
    {
        var command = new RefreshTokenCommand("", "some-refresh-token");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.AccessToken);
    }

    [Fact]
    public void Validate_WithEmptyRefreshToken_Fails()
    {
        var command = new RefreshTokenCommand("some-access-token", "");
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.RefreshToken);
    }
}
