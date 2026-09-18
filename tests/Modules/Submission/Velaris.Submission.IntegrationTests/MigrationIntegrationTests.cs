using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Velaris.Submission.IntegrationTests;

public sealed class MigrationIntegrationTests : IClassFixture<PostgreSqlTestFixture>
{
    private readonly PostgreSqlTestFixture _fixture;

    public MigrationIntegrationTests(PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Migrations_ApplySuccessfully_ToEmptyPostgreSqlDatabase()
    {
        await using var context = _fixture.CreateDbContext();

        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        Assert.Empty(pendingMigrations);

        var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
        Assert.Contains(appliedMigrations, m => m.Contains("InitialSubmissionModel"));
    }

    [Fact]
    public async Task Schema_ContainsAllRequiredTables_InSubmissionSchema()
    {
        await using var context = _fixture.CreateDbContext();
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = 'submission'
            ORDER BY table_name;
            """;

        var tableNames = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tableNames.Add(reader.GetString(0));
        }

        Assert.Contains("tenants", tableNames);
        Assert.Contains("environments", tableNames);
        Assert.Contains("credentials", tableNames);
        Assert.Contains("credential_verifiers", tableNames);
        Assert.Contains("sender_grants", tableNames);
        Assert.Contains("suppressions", tableNames);
        Assert.Contains("tenant_quotas", tableNames);
        Assert.Contains("environment_quotas", tableNames);
        Assert.Contains("messages", tableNames);
        Assert.Contains("message_headers", tableNames);
        Assert.Contains("message_body_parts", tableNames);
        Assert.Contains("attachment_references", tableNames);
        Assert.Contains("deliveries", tableNames);
    }
}
