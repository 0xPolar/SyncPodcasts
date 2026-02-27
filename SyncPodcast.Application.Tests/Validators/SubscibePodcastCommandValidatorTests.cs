using FluentValidation.TestHelper;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

// Note: class name preserves the existing typo in the validator ("Subscibe" instead of "Subscribe")
public class SubscibePodcastCommandValidatorTests
{
    private readonly SubscibePodcastCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_Passes()
    {
        var command = new SubscribePodcastCommand(Guid.NewGuid(), new Uri("https://example.com/feed.xml"));
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var command = new SubscribePodcastCommand(Guid.Empty, new Uri("https://example.com/feed.xml"));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Validate_WithRelativeFeedUrl_Fails()
    {
        // A relative URI is not a well-formed absolute URI, so Must() fails
        var command = new SubscribePodcastCommand(Guid.NewGuid(), new Uri("/relative-path", UriKind.Relative));
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FeedURL);
    }
}
