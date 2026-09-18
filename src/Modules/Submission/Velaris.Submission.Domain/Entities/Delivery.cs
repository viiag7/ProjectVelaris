using Velaris.Submission.Domain.Enums;

namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Independent delivery unit for one accepted recipient of a message (ADR-0002, RF-MSG-006).
/// </summary>
public sealed class Delivery
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid EnvironmentId { get; set; }

    public Guid MessageId { get; set; }

    public string RecipientEmail { get; set; } = string.Empty;

    public DeliveryState State { get; set; } = DeliveryState.Pending;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public Tenant? Tenant { get; set; }

    public TenantEnvironment? Environment { get; set; }

    public Message? Message { get; set; }
}
