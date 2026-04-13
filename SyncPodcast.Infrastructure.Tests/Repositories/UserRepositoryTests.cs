using FluentAssertions;
using SyncPodcast.Domain.Tests.Helpers;
using SyncPodcast.Infrastructure.Repositories;
using SyncPodcast.Infrastructure.Tests.Fixtures;

namespace SyncPodcast.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : IClassFixture<PostgresFixture>
{
    private readonly PostgresFixture _fixture;

    public UserRepositoryTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task AddAsync_ThenGetByIdAsync_ReturnsUser()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new UserRepository(context);
        var user = EntityFactory.CreateUser(username: "adduser", email: "add@test.com");

        await repo.AddAsync(user, CancellationToken.None);
        var fetched = await repo.GetByIdAsync(user.ID, CancellationToken.None);

        fetched.Should().NotBeNull();
        fetched!.Username.Should().Be("adduser");
        fetched.Email.Should().Be("add@test.com");
    }

    [Fact]
    public async Task GetByUsernameAsync_WhenExists_ReturnsUser()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new UserRepository(context);
        var user = EntityFactory.CreateUser(username: $"byname_{Guid.NewGuid():N}", email: $"{Guid.NewGuid():N}@test.com");
        await repo.AddAsync(user, CancellationToken.None);

        var fetched = await repo.GetByUsernameAsync(user.Username, CancellationToken.None);

        fetched.Should().NotBeNull();
        fetched!.ID.Should().Be(user.ID);
    }

    [Fact]
    public async Task GetByUsernameAsync_WhenNotExists_ReturnsNull()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new UserRepository(context);

        var fetched = await repo.GetByUsernameAsync($"ghost_{Guid.NewGuid():N}", CancellationToken.None);

        fetched.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WhenExists_ReturnsUser()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new UserRepository(context);
        var email = $"{Guid.NewGuid():N}@test.com";
        var user = EntityFactory.CreateUser(username: $"byemail_{Guid.NewGuid():N}", email: email);
        await repo.AddAsync(user, CancellationToken.None);

        var fetched = await repo.GetByEmailAsync(email, CancellationToken.None);

        fetched.Should().NotBeNull();
        fetched!.ID.Should().Be(user.ID);
    }

    [Fact]
    public async Task UpdateUserAsync_ModifiesExistingUser()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new UserRepository(context);
        var user = EntityFactory.CreateUser(username: $"upd_{Guid.NewGuid():N}", email: $"{Guid.NewGuid():N}@test.com");
        await repo.AddAsync(user, CancellationToken.None);

        user.ChangePassword("new-hashed-password");
        await repo.UpdateUserAsync(user, CancellationToken.None);

        await using var verifyContext = _fixture.CreateDbContext();
        var verifyRepo = new UserRepository(verifyContext);
        var fetched = await verifyRepo.GetByIdAsync(user.ID, CancellationToken.None);
        fetched!.PasswordHash.Should().Be("new-hashed-password");
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new UserRepository(context);
        var user = EntityFactory.CreateUser(username: $"del_{Guid.NewGuid():N}", email: $"{Guid.NewGuid():N}@test.com");
        await repo.AddAsync(user, CancellationToken.None);

        await repo.DeleteAsync(user.ID, CancellationToken.None);

        var fetched = await repo.GetByIdAsync(user.ID, CancellationToken.None);
        fetched.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenNotExists_DoesNotThrow()
    {
        await using var context = _fixture.CreateDbContext();
        var repo = new UserRepository(context);

        var act = async () => await repo.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        await act.Should().NotThrowAsync();
    }
}
