using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Velaris.Submission.Infrastructure.Persistence;

public sealed class SubmissionDbContextFactory : IDesignTimeDbContextFactory<SubmissionDbContext>
{
    public SubmissionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SubmissionDbContext>();

        // Design-time dummy connection string for migration generation
        optionsBuilder.UseNpgsql("Host=localhost;Database=velaris_submission;Username=postgres;Password=postgres");

        return new SubmissionDbContext(optionsBuilder.Options);
    }
}
