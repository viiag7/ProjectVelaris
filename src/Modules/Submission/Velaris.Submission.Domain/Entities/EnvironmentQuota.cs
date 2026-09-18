using Velaris.Submission.Domain.Enums;

namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Environment-level sending quota window and counter (ADR-0003, RF-QUO-001).
/// </summary>
public sealed class EnvironmentQuota
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid EnvironmentId { get; set; }

    public QuotaPeriodType PeriodType { get; set; }

    public DateTimeOffset PeriodStartUtc { get; set; }

    public DateTimeOffset PeriodEndUtc { get; set; }

    public long AllocatedLimit { get; set; }

    public long UsedCount { get; set; }

    public TenantEnvironment? Environment { get; set; }
}
