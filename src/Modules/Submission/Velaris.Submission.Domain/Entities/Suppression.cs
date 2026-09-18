namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Recipient suppression entry scoped to an Environment (RF-SUP-001, RF-SUP-002, RF-SUP-007).
/// </summary>
public sealed class Suppression
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid EnvironmentId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Reason { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public int? SmtpCode { get; set; }

    public string? EnhancedStatusCode { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? ExpiresAtUtc { get; set; }

    public TenantEnvironment? Environment { get; set; }
}
