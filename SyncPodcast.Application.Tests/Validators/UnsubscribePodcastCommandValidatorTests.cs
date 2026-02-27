using FluentValidation.TestHelper;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class UnsubscribePodcastCommandValidatorTests
{
    private readonly UnsubscribePodcastCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new UnsubscribePodcastCommand(Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var command = new UnsubscribePodcastCommand(Guid.Empty, Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WithEmptyPodcastId_Fails()
    {
        var command = new UnsubscribePodcastCommand(Guid.NewGuid(), Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.PodcastId);
    }
}
