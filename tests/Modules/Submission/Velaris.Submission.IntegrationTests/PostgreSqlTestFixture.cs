using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Velaris.Submission.Infrastructure.Persistence;
using Xunit;

namespace Velaris.Submission.IntegrationTests;

public sealed class PostgreSqlTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine")
        .WithDatabase("velaris_test")
        .WithUsername("velaris")
        .WithPassword("VelarisSecurePass123!")
        .Build();

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        await using var context = CreateDbContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }

    public SubmissionDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SubmissionDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new SubmissionDbContext(options);
    }
}
