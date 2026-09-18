using Velaris.Submission.Domain.Enums;

namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Tenant aggregate root representing the primary isolation boundary.
/// </summary>
public sealed class Tenant
{
    public Guid Id { get; set; }

    public string Slug { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TenantState State { get; set; } = TenantState.Active;

    public long MaxMessageSizeBytes { get; set; }

    public int MaxRecipientsPerSubmission { get; set; }

    public int MaxSmtpSessionDurationSeconds { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public ICollection<TenantEnvironment> Environments { get; set; } = [];

    public ICollection<Credential> Credentials { get; set; } = [];

    public ICollection<Message> Messages { get; set; } = [];

    public ICollection<TenantQuota> Quotas { get; set; } = [];

    /// <summary>
    /// Validates tenant SMTP limits against mandatory platform ceilings (RF-TEN-015).
    /// </summary>
    public void ValidateAgainstCeilings(
        long platformMaxMessageSize,
        int platformMaxRecipients,
        int platformMaxDurationSeconds)
    {
        if (MaxMessageSizeBytes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxMessageSizeBytes),
                MaxMessageSizeBytes,
                "MaxMessageSizeBytes must be greater than zero.");
        }

        if (MaxMessageSizeBytes > platformMaxMessageSize)
        {
            throw new InvalidOperationException(
                $"Tenant max message size ({MaxMessageSizeBytes}) exceeds platform ceiling ({platformMaxMessageSize}).");
        }

        if (MaxRecipientsPerSubmission <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxRecipientsPerSubmission),
                MaxRecipientsPerSubmission,
                "MaxRecipientsPerSubmission must be greater than zero.");
        }

        if (MaxRecipientsPerSubmission > platformMaxRecipients)
        {
            throw new InvalidOperationException(
                $"Tenant max recipients ({MaxRecipientsPerSubmission}) exceeds platform ceiling ({platformMaxRecipients}).");
        }

        if (MaxSmtpSessionDurationSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxSmtpSessionDurationSeconds),
                MaxSmtpSessionDurationSeconds,
                "MaxSmtpSessionDurationSeconds must be greater than zero.");
        }

        if (MaxSmtpSessionDurationSeconds > platformMaxDurationSeconds)
        {
            throw new InvalidOperationException(
                $"Tenant max session duration ({MaxSmtpSessionDurationSeconds}) exceeds platform ceiling ({platformMaxDurationSeconds}).");
        }
    }
}
