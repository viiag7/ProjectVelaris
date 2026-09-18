using Microsoft.Extensions.Options;
using Velaris.SmtpSubmission.Host.Configuration;
using Xunit;

namespace Velaris.Submission.UnitTests;

public sealed class ConfigurationTests
{
    private readonly SmtpSubmissionOptionsValidator _validator = new();

    [Fact]
    public void Validate_WithValidOptions_ReturnsSuccess()
    {
        var options = new SmtpSubmissionOptions
        {
            Port = 465,
            ListenAddress = "127.0.0.1",
            MaxConnections = 500,
            ConnectionTimeoutSeconds = 60,
            TlsCertificatePath = "/path/to/cert.pfx",
            TlsCertificatePassword = "SecretPassword123!"
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(65536)]
    public void Validate_WithInvalidPort_ReturnsFailure(int port)
    {
        var options = new SmtpSubmissionOptions
        {
            Port = port,
            ListenAddress = "127.0.0.1",
            MaxConnections = 500,
            ConnectionTimeoutSeconds = 60,
            TlsCertificatePath = "/path/to/cert.pfx"
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("Port must be between 1 and 65535"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyListenAddress_ReturnsFailure(string listenAddress)
    {
        var options = new SmtpSubmissionOptions
        {
            Port = 465,
            ListenAddress = listenAddress,
            MaxConnections = 500,
            ConnectionTimeoutSeconds = 60,
            TlsCertificatePath = "/path/to/cert.pfx"
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("ListenAddress must not be empty"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Validate_WithInvalidMaxConnections_ReturnsFailure(int maxConnections)
    {
        var options = new SmtpSubmissionOptions
        {
            Port = 465,
            ListenAddress = "127.0.0.1",
            MaxConnections = maxConnections,
            ConnectionTimeoutSeconds = 60,
            TlsCertificatePath = "/path/to/cert.pfx"
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("MaxConnections must be greater than 0"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_WithInvalidTimeout_ReturnsFailure(int timeout)
    {
        var options = new SmtpSubmissionOptions
        {
            Port = 465,
            ListenAddress = "127.0.0.1",
            MaxConnections = 500,
            ConnectionTimeoutSeconds = timeout,
            TlsCertificatePath = "/path/to/cert.pfx"
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("ConnectionTimeoutSeconds must be greater than 0"));
    }

    [Fact]
    public void Validate_WithEmptyTlsCertificatePath_ReturnsFailure()
    {
        var options = new SmtpSubmissionOptions
        {
            Port = 465,
            ListenAddress = "127.0.0.1",
            MaxConnections = 500,
            ConnectionTimeoutSeconds = 60,
            TlsCertificatePath = ""
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("TlsCertificatePath is required"));
    }

    [Fact]
    public void Validate_FailureMessages_NeverExposeSecretValues()
    {
        const string secretPassword = "SuperSecretPasswordDoNotLog!";
        var options = new SmtpSubmissionOptions
        {
            Port = -1,
            ListenAddress = "",
            MaxConnections = 0,
            ConnectionTimeoutSeconds = 0,
            TlsCertificatePath = "",
            TlsCertificatePassword = secretPassword
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        foreach (var failure in result.Failures)
        {
            Assert.DoesNotContain(secretPassword, failure);
        }
    }
}
