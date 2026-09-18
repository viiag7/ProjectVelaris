using Velaris.Submission.Domain.Enums;

namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Tenant-level aggregate sending quota window and counter (ADR-0003, RF-QUO-002).
/// </summary>
public sealed class TenantQuota
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public QuotaPeriodType PeriodType { get; set; }

    public DateTimeOffset PeriodStartUtc { get; set; }

    public DateTimeOffset PeriodEndUtc { get; set; }

    public long AllocatedLimit { get; set; }

    public long UsedCount { get; set; }

    public Tenant? Tenant { get; set; }
}
