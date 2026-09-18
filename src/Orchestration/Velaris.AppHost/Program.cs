var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Velaris_SmtpSubmission_Host>("smtp-submission");

builder.Build().Run();
