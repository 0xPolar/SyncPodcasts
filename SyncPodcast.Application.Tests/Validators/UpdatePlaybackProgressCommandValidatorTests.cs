using FluentValidation.TestHelper;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class UpdatePlaybackProgressCommandValidatorTests
{
    private readonly UpdatePlaybackProgressCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new UpdatePlaybackProgressCommand(Guid.NewGuid(), Guid.NewGuid(), TimeSpan.FromMinutes(10));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithZeroProgress_Passes()
    {
        // GreaterThanOrEqualTo(TimeSpan.Zero) — zero is explicitly allowed
        var command = new UpdatePlaybackProgressCommand(Guid.NewGuid(), Guid.NewGuid(), TimeSpan.Zero);
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var command = new UpdatePlaybackProgressCommand(Guid.Empty, Guid.NewGuid(), TimeSpan.FromMinutes(10));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WithEmptyEpisodeId_Fails()
    {
        var command = new UpdatePlaybackProgressCommand(Guid.NewGuid(), Guid.Empty, TimeSpan.FromMinutes(10));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.EpisodeId);
    }

    [Fact]
    public void Validate_WithNegativeProgress_Fails()
    {
        var command = new UpdatePlaybackProgressCommand(Guid.NewGuid(), Guid.NewGuid(), TimeSpan.FromSeconds(-1));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Progress);
    }
}
