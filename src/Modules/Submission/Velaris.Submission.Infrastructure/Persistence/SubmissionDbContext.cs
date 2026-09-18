using Microsoft.EntityFrameworkCore;
using Velaris.Submission.Domain.Entities;

namespace Velaris.Submission.Infrastructure.Persistence;

public class SubmissionDbContext(DbContextOptions<SubmissionDbContext> options) : DbContext(options)
{
    public const string SchemaName = "submission";

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<TenantEnvironment> Environments => Set<TenantEnvironment>();

    public DbSet<Credential> Credentials => Set<Credential>();

    public DbSet<CredentialVerifier> CredentialVerifiers => Set<CredentialVerifier>();

    public DbSet<SenderGrant> SenderGrants => Set<SenderGrant>();

    public DbSet<Suppression> Suppressions => Set<Suppression>();

    public DbSet<TenantQuota> TenantQuotas => Set<TenantQuota>();

    public DbSet<EnvironmentQuota> EnvironmentQuotas => Set<EnvironmentQuota>();

    public DbSet<Message> Messages => Set<Message>();

    public DbSet<MessageHeader> MessageHeaders => Set<MessageHeader>();

    public DbSet<MessageBodyPart> MessageBodyParts => Set<MessageBodyPart>();

    public DbSet<AttachmentReference> AttachmentReferences => Set<AttachmentReference>();

    public DbSet<Delivery> Deliveries => Set<Delivery>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaName);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SubmissionDbContext).Assembly);
    }
}
