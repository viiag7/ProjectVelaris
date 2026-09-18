using Microsoft.Extensions.Options;

namespace Velaris.SmtpSubmission.Host.Configuration;

/// <summary>
/// Validates SMTP submission options at application startup without exposing secret values in failure messages.
/// </summary>
public sealed class SmtpSubmissionOptionsValidator : IValidateOptions<SmtpSubmissionOptions>
{
    public ValidateOptionsResult Validate(string? name, SmtpSubmissionOptions options)
    {
        var failures = new List<string>();

        if (options.Port is < 1 or > 65535)
        {
            failures.Add($"Port must be between 1 and 65535, but was {options.Port}.");
        }

        if (string.IsNullOrWhiteSpace(options.ListenAddress))
        {
            failures.Add("ListenAddress must not be empty.");
        }

        if (options.MaxConnections <= 0)
        {
            failures.Add($"MaxConnections must be greater than 0, but was {options.MaxConnections}.");
        }

        if (options.ConnectionTimeoutSeconds <= 0)
        {
            failures.Add($"ConnectionTimeoutSeconds must be greater than 0, but was {options.ConnectionTimeoutSeconds}.");
        }

        if (string.IsNullOrWhiteSpace(options.TlsCertificatePath))
        {
            failures.Add("TlsCertificatePath is required and must not be empty.");
        }

        // Note: TlsCertificatePassword must never be included in failure messages or logged.

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
