using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Velaris.SmtpSubmission.Host;
using Velaris.SmtpSubmission.Host.Configuration;
using Xunit;

namespace Velaris.Submission.UnitTests;

public sealed class HostLifecycleTests
{
    [Fact]
    public async Task Host_StartsAndStopsGracefully_OnCancellation()
    {
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["SmtpSubmission:Port"] = "10465",
            ["SmtpSubmission:ListenAddress"] = "127.0.0.1",
            ["SmtpSubmission:MaxConnections"] = "100",
            ["SmtpSubmission:ConnectionTimeoutSeconds"] = "30",
            ["SmtpSubmission:TlsCertificatePath"] = "certs/test-cert.pfx"
        };

        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(inMemoryConfig);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IValidateOptions<SmtpSubmissionOptions>, SmtpSubmissionOptionsValidator>();
                services.AddOptions<SmtpSubmissionOptions>()
                    .Bind(context.Configuration.GetSection(SmtpSubmissionOptions.SectionName))
                    .ValidateOnStart();
                services.AddHostedService<SubmissionHostWorker>();
            });

        using var host = hostBuilder.Build();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));

        // StartAsync with cancellation token and verify it starts and stops cleanly
        await host.StartAsync(cts.Token);

        // Signal stop gracefully
        using var stopCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        await host.StopAsync(stopCts.Token);
    }

    [Fact]
    public void Host_ThrowsOptionsValidationException_WhenConfigurationInvalidOnStart()
    {
        var inMemoryConfig = new Dictionary<string, string?>
        {
            ["SmtpSubmission:Port"] = "-1",
            ["SmtpSubmission:ListenAddress"] = "",
            ["SmtpSubmission:TlsCertificatePath"] = ""
        };

        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(inMemoryConfig);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IValidateOptions<SmtpSubmissionOptions>, SmtpSubmissionOptionsValidator>();
                services.AddOptions<SmtpSubmissionOptions>()
                    .Bind(context.Configuration.GetSection(SmtpSubmissionOptions.SectionName))
                    .ValidateOnStart();
                services.AddHostedService<SubmissionHostWorker>();
            });

        using var host = hostBuilder.Build();

        var exception = Assert.Throws<OptionsValidationException>(() =>
        {
            _ = host.Services.GetRequiredService<IOptions<SmtpSubmissionOptions>>().Value;
        });

        Assert.NotEmpty(exception.Failures);
    }
}
