using Velaris.Submission.Domain.Enums;

namespace Velaris.Submission.Domain.Entities;

/// <summary>
/// Credential used to authenticate submission sessions.
/// Does not store recoverable passwords (ADR-0009, ADR-0015).
/// </summary>
public sealed class Credential
{
    public Guid Id { get; set; }

    public Guid TenantId { get; set; }

    public Guid EnvironmentId { get; set; }

    public string Name { get; set; } = string.Empty;

    public CredentialType Type { get; set; } = CredentialType.Smtp;

    public CredentialState State { get; set; } = CredentialState.Active;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }

    public Tenant? Tenant { get; set; }

    public TenantEnvironment? Environment { get; set; }

    public ICollection<CredentialVerifier> Verifiers { get; set; } = [];

    public ICollection<SenderGrant> SenderGrants { get; set; } = [];

    /// <summary>
    /// Adds a new verifier version and deactivates any existing active verifier (rotate lifecycle).
    /// </summary>
    public CredentialVerifier AddVerifier(
        string mechanism,
        byte[] salt,
        int iterationCount,
        byte[] storedKey,
        byte[] serverKey,
        DateTimeOffset timestampUtc)
    {
        if (State == CredentialState.Revoked)
        {
            throw new InvalidOperationException("Cannot add a verifier to a revoked credential.");
        }

        var nextVersion = Verifiers.Count == 0 ? 1 : Verifiers.Max(v => v.Version) + 1;

        foreach (var verifier in Verifiers.Where(v => v.IsActive))
        {
            verifier.IsActive = false;
        }

        var newVerifier = new CredentialVerifier
        {
            Id = Guid.NewGuid(),
            TenantId = TenantId,
            CredentialId = Id,
            Version = nextVersion,
            IsActive = true,
            Mechanism = mechanism,
            Salt = salt,
            IterationCount = iterationCount,
            StoredKey = storedKey,
            ServerKey = serverKey,
            CreatedAtUtc = timestampUtc
        };

        Verifiers.Add(newVerifier);
        return newVerifier;
    }

    /// <summary>
    /// Revokes the credential and deactivates all associated verifier versions.
    /// </summary>
    public void Revoke(DateTimeOffset timestampUtc)
    {
        State = CredentialState.Revoked;
        RevokedAtUtc = timestampUtc;

        foreach (var verifier in Verifiers)
        {
            verifier.IsActive = false;
            verifier.RevokedAtUtc ??= timestampUtc;
        }
    }
}
