using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Velaris.Submission.AspireTests;

public sealed class SubmissionAppHostTests
{
    [Fact]
    public async Task AppHost_StartsSubmissionServiceSuccessfully()
    {
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.Velaris_AppHost>();

        await using var app = await appHost.BuildAsync();
        await app.StartAsync();

        var notificationService = app.Services.GetRequiredService<ResourceNotificationService>();
        await notificationService.WaitForResourceAsync("smtp-submission", KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromSeconds(30));
    }
}
