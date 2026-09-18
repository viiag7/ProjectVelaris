namespace Velaris.SmtpSubmission.Host.Configuration;

/// <summary>
/// Strongly typed configuration options for the SMTP submission service.
/// </summary>
public sealed class SmtpSubmissionOptions
{
    public const string SectionName = "SmtpSubmission";

    public int Port { get; init; } = 465;

    public string ListenAddress { get; init; } = "0.0.0.0";

    public int MaxConnections { get; init; } = 1000;

    public int ConnectionTimeoutSeconds { get; init; } = 120;

    public string TlsCertificatePath { get; init; } = string.Empty;

    public string? TlsCertificatePassword { get; init; }
}
