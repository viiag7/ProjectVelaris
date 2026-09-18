using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Velaris.SmtpSubmission.Host.Configuration;

namespace Velaris.SmtpSubmission.Host;

/// <summary>
/// Background worker managing the lifecycle of the SMTP submission host.
/// </summary>
public sealed class SubmissionHostWorker(
    ILogger<SubmissionHostWorker> logger,
    IOptions<SmtpSubmissionOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        logger.LogInformation(
            "Starting SMTP Submission service listening on {ListenAddress}:{Port}",
            settings.ListenAddress,
            settings.Port);

        try
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("SMTP Submission service shutdown requested; stopping gracefully");
        }
    }
}
