using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Velaris.SmtpSubmission.Host;
using Velaris.SmtpSubmission.Host.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSingleton<IValidateOptions<SmtpSubmissionOptions>, SmtpSubmissionOptionsValidator>();

builder.Services.AddOptions<SmtpSubmissionOptions>()
    .BindConfiguration(SmtpSubmissionOptions.SectionName)
    .ValidateOnStart();

builder.Services.AddHostedService<SubmissionHostWorker>();

var host = builder.Build();

await host.RunAsync();

public partial class Program;
