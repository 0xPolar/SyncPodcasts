using FluentAssertions;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class SearchPodcastQueryValidatorTests
{
    private readonly SearchPodcastQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_Passes()
    {
        var query = new SearchPodcastQuery("software engineering", 1, 10);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyQuery_Fails()
    {
        var query = new SearchPodcastQuery("", 1, 10);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.Query));
    }

    [Fact]
    public void Validate_WithZeroPage_Fails()
    {
        var query = new SearchPodcastQuery("software", 0, 10);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.Page));
    }

    [Fact]
    public void Validate_WithZeroPageSize_Fails()
    {
        var query = new SearchPodcastQuery("software", 1, 0);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.PageSize));
    }
}
