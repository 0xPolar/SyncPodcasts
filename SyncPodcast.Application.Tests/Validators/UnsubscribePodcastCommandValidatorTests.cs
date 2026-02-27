using FluentAssertions;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class UnsubscribePodcastCommandValidatorTests
{
    private readonly UnsubscribePodcastCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new UnsubscribePodcastCommand(Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.Validate(command);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var command = new UnsubscribePodcastCommand(Guid.Empty, Guid.NewGuid());
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.UserId));
    }

    [Fact]
    public void Validate_WithEmptyPodcastId_Fails()
    {
        var command = new UnsubscribePodcastCommand(Guid.NewGuid(), Guid.Empty);
        var result = _validator.Validate(command);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(command.PodcastId));
    }
}
