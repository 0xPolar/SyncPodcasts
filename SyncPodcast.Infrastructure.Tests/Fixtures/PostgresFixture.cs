using Testcontainers.PostgreSql;
using Microsoft.EntityFrameworkCore;
using SyncPodcast.Infrastructure.Persistence;

namespace SyncPodcast.Infrastructure.Tests.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        //.WithDatabase("syncpodcast_test")
        //.WithUsername("postgres")
        //.WithPassword("postgres")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public SyncPodcastDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SyncPodcastDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;
        return new SyncPodcastDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        using var context = CreateDbContext();

        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
