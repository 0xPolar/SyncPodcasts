  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Testing;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.DependencyInjection;
  using Testcontainers.PostgreSql;
  using SyncPodcast.Infrastructure.Persistence;

  namespace SyncPodcast.API.Tests.Fixtures;

  public class SyncPodcastApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
  {
      private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:16-alpine")
          .Build();

      protected override void ConfigureWebHost(IWebHostBuilder builder)
      {
          builder.ConfigureServices(services =>
          {
              // Remove the existing DbContext registration
              var descriptor = services.SingleOrDefault(
                  d => d.ServiceType == typeof(DbContextOptions<SyncPodcastDbContext>));
              if (descriptor != null)
                  services.Remove(descriptor);

              // Add DbContext with TestContainers connection string
              services.AddDbContext<SyncPodcastDbContext>(options =>
                  options.UseNpgsql(_container.GetConnectionString()));
          });
      }

      public async Task InitializeAsync()
      {
          await _container.StartAsync();

          // Ensure database schema exists
          using var scope = Services.CreateScope();
          var db = scope.ServiceProvider.GetRequiredService<SyncPodcastDbContext>();
          await db.Database.EnsureCreatedAsync();
      }

      public new async Task DisposeAsync()
      {
          await _container.DisposeAsync();
          await base.DisposeAsync();
      }
  }
