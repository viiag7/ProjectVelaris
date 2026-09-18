using Velaris.Submission.Domain.Enums;

namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Logical Environment owned by a Tenant (e.g. dev, staging, prod).
/// </summary>
public sealed class TenantEnvironment
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public EnvironmentState State { get; set; } = EnvironmentState.Active;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public Tenant? Tenant { get; set; }

    public ICollection<Credential> Credentials { get; set; } = [];

    public ICollection<Suppression> Suppressions { get; set; } = [];

    public ICollection<EnvironmentQuota> Quotas { get; set; } = [];
}
