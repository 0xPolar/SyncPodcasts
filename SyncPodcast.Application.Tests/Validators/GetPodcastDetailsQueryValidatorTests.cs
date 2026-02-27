using FluentAssertions;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class GetPodcastDetailsQueryValidatorTests
{
    private readonly GetPodcastDetailsQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_Passes()
    {
        var query = new GetPodcastDetailsQuery(Guid.NewGuid());
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyPodcastId_Fails()
    {
        var query = new GetPodcastDetailsQuery(Guid.Empty);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.PodcastId));
    }
}
