using FluentAssertions;
using SyncPodcast.Application.CQRS;

namespace SyncPodcast.Application.Tests.Validators;

public class GetLibraryQueryValidatorTests
{
    private readonly GetLibraryQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_Passes()
    {
        var query = new GetLibraryQuery(Guid.NewGuid());
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyUserId_Fails()
    {
        var query = new GetLibraryQuery(Guid.Empty);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(query.UserId));
    }
}
