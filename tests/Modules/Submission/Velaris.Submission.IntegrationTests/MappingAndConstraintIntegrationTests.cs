using Microsoft.EntityFrameworkCore;
using Velaris.Submission.Domain.Entities;
using Velaris.Submission.Domain.Enums;
using Xunit;

namespace Velaris.Submission.IntegrationTests;

public sealed class MappingAndConstraintIntegrationTests : IClassFixture<PostgreSqlTestFixture>
{
    private readonly PostgreSqlTestFixture _fixture;

    public MappingAndConstraintIntegrationTests(PostgreSqlTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Tenant_PersistsWithSmtpLimits_AndValidatesCeilings()
    {
        await using var context = _fixture.CreateDbContext();

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Slug = $"tenant-{Guid.NewGuid():N}",
            Name = "Acme Corp",
            State = TenantState.Active,
            MaxMessageSizeBytes = 25 * 1024 * 1024,
            MaxRecipientsPerSubmission = 50,
            MaxSmtpSessionDurationSeconds = 300,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };

        // Domain validation against platform ceilings
        tenant.ValidateAgainstCeilings(
            platformMaxMessageSize: 50 * 1024 * 1024,
            platformMaxRecipients: 100,
            platformMaxDurationSeconds: 600);

        context.Tenants.Add(tenant);
        await context.SaveChangesAsync();

        var persisted = await context.Tenants.FindAsync(tenant.Id);
        Assert.NotNull(persisted);
        Assert.Equal("Acme Corp", persisted.Name);
        Assert.Equal(25 * 1024 * 1024, persisted.MaxMessageSizeBytes);
    }

    [Fact]
    public async Task Credential_SupportsCreateRotateRevoke_WithoutPlainPassword()
    {
        await using var context = _fixture.CreateDbContext();

        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Slug = $"tenant-cred-{Guid.NewGuid():N}",
            Name = "Cred Tenant",
            State = TenantState.Active,
            MaxMessageSizeBytes = 10 * 1024 * 1024,
            MaxRecipientsPerSubmission = 20,
            MaxSmtpSessionDurationSeconds = 120,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        context.Tenants.Add(tenant);

        var env = new TenantEnvironment
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Name = "production",
            State = EnvironmentState.Active,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        context.Environments.Add(env);

        var credential = new Credential
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            EnvironmentId = env.Id,
            Name = "smtp-mailer",
            Type = CredentialType.Smtp,
            State = CredentialState.Active,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        var now = DateTimeOffset.UtcNow;
        var v1 = credential.AddVerifier("SCRAM-SHA-256", [1, 2, 3], 4096, [10, 20], [30, 40], now);
        Assert.Equal(1, v1.Version);
        Assert.True(v1.IsActive);

        // Rotate: activates version 2 and deactivates version 1
        var v2 = credential.AddVerifier("SCRAM-SHA-256", [4, 5, 6], 4096, [50, 60], [70, 80], now.AddMinutes(5));
        Assert.Equal(2, v2.Version);
        Assert.True(v2.IsActive);
        Assert.False(v1.IsActive);

        context.Credentials.Add(credential);
        await context.SaveChangesAsync();

        var loaded = await context.Credentials
            .Include(c => c.Verifiers)
            .FirstAsync(c => c.Id == credential.Id);

        Assert.Equal(2, loaded.Verifiers.Count);
        Assert.Single(loaded.Verifiers, v => v.IsActive);
        Assert.Single(loaded.Verifiers, v => !v.IsActive);

        // Revoke: marks all inactive and revoked
        loaded.Revoke(DateTimeOffset.UtcNow);
        await context.SaveChangesAsync();

        var revoked = await context.Credentials
            .Include(c => c.Verifiers)
            .FirstAsync(c => c.Id == credential.Id);

        Assert.Equal(CredentialState.Revoked, revoked.State);
        Assert.All(revoked.Verifiers, v => Assert.False(v.IsActive));
    }

    [Fact]
    public async Task Message_PreservesOrderedHeadersBodyPartsAndAttachments()
    {
        await using var context = _fixture.CreateDbContext();

        var tenantId = Guid.NewGuid();
        var envId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var tenant = new Tenant
        {
            Id = tenantId,
            Slug = $"tenant-msg-{Guid.NewGuid():N}",
            Name = "Msg Tenant",
            State = TenantState.Active,
            MaxMessageSizeBytes = 10 * 1024 * 1024,
            MaxRecipientsPerSubmission = 10,
            MaxSmtpSessionDurationSeconds = 60,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        var env = new TenantEnvironment
        {
            Id = envId,
            TenantId = tenantId,
            Name = "staging",
            State = EnvironmentState.Active,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        context.Tenants.Add(tenant);
        context.Environments.Add(env);

        var message = new Message
        {
            Id = messageId,
            TenantId = tenantId,
            EnvironmentId = envId,
            EnvelopeSender = "sender@example.com",
            Subject = "Test Message",
            AcceptedAtUtc = DateTimeOffset.UtcNow,
            SubmissionIp = "192.168.1.100",
            ClientEhlo = "mail.example.com",
            TlsCipher = "TLS_AES_256_GCM_SHA384",
            AuthenticatedUser = "smtp-user"
        };

        // Repeated and ordered headers (ADR-0010)
        message.Headers.Add(new MessageHeader
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MessageId = messageId,
            OrderIndex = 0,
            Name = "Received",
            Value = "from client.example.com"
        });
        message.Headers.Add(new MessageHeader
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MessageId = messageId,
            OrderIndex = 1,
            Name = "Received",
            Value = "from edge.example.com"
        });
        message.Headers.Add(new MessageHeader
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MessageId = messageId,
            OrderIndex = 2,
            Name = "Subject",
            Value = "Test Message"
        });

        // Body parts
        message.BodyParts.Add(new MessageBodyPart
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MessageId = messageId,
            OrderIndex = 0,
            MediaType = "text/plain",
            Charset = "utf-8",
            Content = "Hello World!"
        });

        // Attachment reference
        message.Attachments.Add(new AttachmentReference
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            MessageId = messageId,
            OpaqueObjectKey = $"tenants/{tenantId}/attachments/{Guid.NewGuid():N}.bin",
            FileName = "invoice.pdf",
            ContentType = "application/pdf",
            SizeBytes = 10240,
            HashAlgorithm = "SHA-256",
            HashValue = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855",
            OrderIndex = 0,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        // Deliveries
        message.Deliveries.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EnvironmentId = envId,
            MessageId = messageId,
            RecipientEmail = "recipient1@example.com",
            State = DeliveryState.Pending,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });
        message.Deliveries.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EnvironmentId = envId,
            MessageId = messageId,
            RecipientEmail = "recipient2@example.com",
            State = DeliveryState.Pending,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        context.Messages.Add(message);
        await context.SaveChangesAsync();

        var loaded = await context.Messages
            .Include(m => m.Headers)
            .Include(m => m.BodyParts)
            .Include(m => m.Attachments)
            .Include(m => m.Deliveries)
            .FirstAsync(m => m.Id == messageId);

        Assert.Equal(3, loaded.Headers.Count);
        var orderedHeaders = loaded.Headers.OrderBy(h => h.OrderIndex).ToList();
        Assert.Equal("Received", orderedHeaders[0].Name);
        Assert.Equal("from client.example.com", orderedHeaders[0].Value);
        Assert.Equal("Received", orderedHeaders[1].Name);
        Assert.Equal("from edge.example.com", orderedHeaders[1].Value);

        Assert.Single(loaded.BodyParts);
        Assert.Single(loaded.Attachments);
        Assert.Equal("invoice.pdf", loaded.Attachments.First().FileName);
        Assert.Equal("SHA-256", loaded.Attachments.First().HashAlgorithm);

        Assert.Equal(2, loaded.Deliveries.Count);
    }

    [Fact]
    public async Task Delivery_EnforcesUniqueRecipientPerMessage()
    {
        await using var context = _fixture.CreateDbContext();

        var tenantId = Guid.NewGuid();
        var envId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var tenant = new Tenant
        {
            Id = tenantId,
            Slug = $"tenant-dup-{Guid.NewGuid():N}",
            Name = "Dup Tenant",
            State = TenantState.Active,
            MaxMessageSizeBytes = 1024,
            MaxRecipientsPerSubmission = 10,
            MaxSmtpSessionDurationSeconds = 60,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        var env = new TenantEnvironment
        {
            Id = envId,
            TenantId = tenantId,
            Name = "dev",
            State = EnvironmentState.Active,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        var message = new Message
        {
            Id = messageId,
            TenantId = tenantId,
            EnvironmentId = envId,
            EnvelopeSender = "sender@example.com",
            AcceptedAtUtc = DateTimeOffset.UtcNow
        };

        context.Tenants.Add(tenant);
        context.Environments.Add(env);
        context.Messages.Add(message);

        context.Deliveries.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EnvironmentId = envId,
            MessageId = messageId,
            RecipientEmail = "user@example.com",
            State = DeliveryState.Pending,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        // Duplicate delivery for same message and recipient
        context.Deliveries.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            EnvironmentId = envId,
            MessageId = messageId,
            RecipientEmail = "user@example.com",
            State = DeliveryState.Pending,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    public async Task Quotas_RejectNegativeAllocatedLimits_ViaDatabaseConstraint()
    {
        await using var context = _fixture.CreateDbContext();

        var tenantId = Guid.NewGuid();
        var tenant = new Tenant
        {
            Id = tenantId,
            Slug = $"tenant-q-{Guid.NewGuid():N}",
            Name = "Quota Tenant",
            State = TenantState.Active,
            MaxMessageSizeBytes = 1024,
            MaxRecipientsPerSubmission = 10,
            MaxSmtpSessionDurationSeconds = 60,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            UpdatedAtUtc = DateTimeOffset.UtcNow
        };
        context.Tenants.Add(tenant);

        var quota = new TenantQuota
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            PeriodType = QuotaPeriodType.Daily,
            PeriodStartUtc = DateTimeOffset.UtcNow,
            PeriodEndUtc = DateTimeOffset.UtcNow.AddDays(1),
            AllocatedLimit = -500, // Negative limit violates check constraint
            UsedCount = 0
        };
        context.TenantQuotas.Add(quota);

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }
}
