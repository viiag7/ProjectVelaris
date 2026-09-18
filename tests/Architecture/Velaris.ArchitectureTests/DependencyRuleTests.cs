using System.Reflection;
using Velaris.Submission.Application;
using Velaris.Submission.Domain;
using Velaris.Submission.Infrastructure;
using Velaris.Submission.Smtp;
using Xunit;

namespace Velaris.ArchitectureTests;

public sealed class DependencyRuleTests
{
    [Fact]
    public void Domain_MustNotReference_Application_Infrastructure_Smtp_Or_Host()
    {
        var domainAssembly = typeof(SubmissionDomainMarker).Assembly;
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies().Select(a => a.Name).ToList();

        Assert.DoesNotContain(referencedAssemblies, name => name is not null && (
            name.StartsWith("Velaris.Submission.Application", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Velaris.Submission.Infrastructure", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Velaris.Submission.Smtp", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Velaris.SmtpSubmission.Host", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Npgsql", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("AWSSDK", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("OpenTelemetry", StringComparison.OrdinalIgnoreCase)
        ));
    }

    [Fact]
    public void Application_MustNotReference_Infrastructure_Smtp_Or_Host()
    {
        var appAssembly = typeof(SubmissionApplicationMarker).Assembly;
        var referencedAssemblies = appAssembly.GetReferencedAssemblies().Select(a => a.Name).ToList();

        Assert.DoesNotContain(referencedAssemblies, name => name is not null && (
            name.StartsWith("Velaris.Submission.Infrastructure", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Velaris.Submission.Smtp", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Velaris.SmtpSubmission.Host", StringComparison.OrdinalIgnoreCase)
        ));
    }

    [Fact]
    public void Infrastructure_MustNotReference_Host_Or_Smtp()
    {
        var infraAssembly = typeof(SubmissionInfrastructureMarker).Assembly;
        var referencedAssemblies = infraAssembly.GetReferencedAssemblies().Select(a => a.Name).ToList();

        Assert.DoesNotContain(referencedAssemblies, name => name is not null && (
            name.StartsWith("Velaris.SmtpSubmission.Host", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Velaris.Submission.Smtp", StringComparison.OrdinalIgnoreCase)
        ));
    }

    [Fact]
    public void Smtp_MustNotReference_Host_Or_Infrastructure()
    {
        var smtpAssembly = typeof(SubmissionSmtpMarker).Assembly;
        var referencedAssemblies = smtpAssembly.GetReferencedAssemblies().Select(a => a.Name).ToList();

        Assert.DoesNotContain(referencedAssemblies, name => name is not null && (
            name.StartsWith("Velaris.SmtpSubmission.Host", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("Velaris.Submission.Infrastructure", StringComparison.OrdinalIgnoreCase)
        ));
    }
}
