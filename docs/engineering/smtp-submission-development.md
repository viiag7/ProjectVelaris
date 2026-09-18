# SMTP Submission Development Guide

This guide turns Story #4, ADR-0014 and ADR-0015 into an executable development baseline. Requirements and accepted ADRs remain authoritative.

## Technology baseline

| Concern | Initial technology | Rule |
|---|---|---|
| Runtime | .NET 10 LTS, `net10.0`, C# 14 | Pin the SDK feature band in `global.json`; deploy on a supported Linux runtime image. |
| Service host | .NET Generic Host / Worker | Own startup, configuration validation, dependency injection, cancellation and graceful shutdown. |
| SMTP transport | .NET 10 TLS/I/O APIs plus the engine selected by the SMTP/SCRAM Spike | Require implicit TLS and server-side `SCRAM-SHA-256`; do not use a package that exposes only `PLAIN`/`LOGIN`. |
| MIME | MimeKit 4.x | Parse bounded streams and map to the relational Message/body/attachment representation without silently discarding valid parts. |
| Database | PostgreSQL 18.x | Authoritative relational source for submission configuration and acceptance state. |
| Database access | Entity Framework Core 10.x + Npgsql EF provider 10.x | EF Core owns mapping and the unit of work. Reviewed parameterized SQL is allowed inside the persistence adapter for exact quota/concurrency or set-based operations. |
| Migrations | Entity Framework Core migrations | Migrations are forward-safe, generate reviewed SQL, are repeatably tested and execute separately from normal service startup in production. |
| Attachment boundary | `IAttachmentStorage` in the application boundary | Provider SDKs stay in adapters; the production provider remains open. MinIO may be used only as an S3-compatible local/CI contract-test target. |
| Observability | OpenTelemetry .NET 1.x + OTLP; `Microsoft.Extensions.Logging` | Trace, metrics and structured-log correlation begin at connection acceptance; never record secrets or message content. |
| Tests | xUnit 3.x, Microsoft Testing Platform, Testcontainers for .NET 4.x | Unit, protocol, PostgreSQL, storage-contract, concurrency and failure tests are required according to task scope. |
| Delivery | Docker multi-stage build + GitHub Actions | Run restore, format verification, build, tests, vulnerability checks and container build. |

Exact package patches are selected and centrally pinned by the bootstrap Task. Stable patch/security upgrades within these lines do not change the architecture.

## Deliberately excluded initially

- ASP.NET MVC or a web API framework in the SMTP data path;
- repository abstractions that merely duplicate EF Core without protecting a domain or application boundary;
- Redis or another configuration/quota cache;
- a broker or queue dependency before the Story #4 acceptance boundary;
- cloud-provider types in domain or application projects;
- automatic client-retry deduplication based on `Message-ID` or content hashes;
- `AUTH PLAIN` or `AUTH LOGIN` fallback.

## Solution shape

The initial repository layout should make dependency direction visible:

```text
src/
  Velaris.Submission.Host/             executable Generic Host
  Velaris.Submission.Application/      use cases, ports and acceptance policy
  Velaris.Submission.Domain/           Message, Delivery and invariant types
  Velaris.Submission.Smtp/             SMTP session and SCRAM adapters
  Velaris.Submission.Persistence/      EF Core, migrations, Npgsql and acceptance transactions
  Velaris.Submission.Attachments/      attachment-storage adapters
tests/
  Velaris.Submission.UnitTests/
  Velaris.Submission.IntegrationTests/
  Velaris.Submission.ProtocolTests/
deploy/
```

Projects may be consolidated when a boundary has no independent value, but infrastructure dependencies must point inward through application interfaces. The domain must not reference Entity Framework Core, Npgsql, an Object Storage SDK, OpenTelemetry or the SMTP library.

## Developer workflow

The bootstrap Task must make these commands sufficient for a clean checkout:

```text
dotnet restore --locked-mode
dotnet format --verify-no-changes
dotnet build --no-restore --configuration Release
dotnet test --no-build --configuration Release
dotnet list package --vulnerable --include-transitive
```

Integration tests start isolated PostgreSQL and storage test dependencies through Testcontainers. Tests must not depend on a developer's shared database or cloud account.

## Implementation order

1. Run the .NET 10 SMTP/SCRAM engine Spike and record the build, extend or library recommendation.
2. Bootstrap the solution, CI, container, configuration validation and telemetry skeleton.
3. Add the EF Core model, versioned PostgreSQL migrations and data-access contracts.
4. Implement the proven TLS/SMTP/SCRAM session path.
5. Add sender, recipient, Suppression and centralized SMTP-reply policies.
6. Parse Message content and durably store attachments behind `IAttachmentStorage`.
7. Implement atomic Message, `N` Deliveries and exact quota acceptance using the result of Spike #6.
8. Prove the complete acceptance and failure matrix with protocol and integration tests.

Tasks that depend on either Spike must state that dependency and may not mark the affected completion criteria done without its evidence.

## Pull-request contract

Every implementation pull request references:

- its Technical Task;
- Story #4;
- applicable RF, RNF and ADR identifiers;
- tests or benchmark evidence;
- migration, security, tenant-isolation, failure and rollback implications.

One pull request should close one coherent Technical Task unless an explicitly documented dependency makes a combined change safer.

## Open decisions

The following remain evidence-driven rather than developer preference:

- SMTP engine and its extension/maintenance strategy;
- final PostgreSQL transaction isolation and quota statement shape, through Spike #6;
- physical indexes and connection-pool size;
- platform hard-limit numeric values, through security/capacity review;
- production Object Storage provider;
- retry delay and jitter values;
- infrastructure sizing.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- Spike #6 — Benchmark concurrent hierarchical quota acceptance
- ADR-0008 through ADR-0015
- [SMTP Submission architecture](../architecture/smtp-submission.md)
- [SMTP Submission implementation guidelines](../architecture/smtp-submission-implementation-guidelines.md)
