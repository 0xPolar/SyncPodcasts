using FluentValidation.TestHelper;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class SearchPodcastQueryValidatorTests
{
    private readonly SearchPodcastQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_Passes()
    {
        var query = new SearchPodcastQuery("software engineering", 1, 10);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyQuery_Fails()
    {
        var query = new SearchPodcastQuery("", 1, 10);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Query);
    }

    [Fact]
    public void Validate_WithZeroPage_Fails()
    {
        var query = new SearchPodcastQuery("software", 0, 10);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WithZeroPageSize_Fails()
    {
        var query = new SearchPodcastQuery("software", 1, 0);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
