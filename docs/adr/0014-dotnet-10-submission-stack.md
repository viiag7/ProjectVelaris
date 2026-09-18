# ADR-0014: SMTP Submission Uses .NET 10

**Status:** Accepted

## Context

Story #4 needs an explicit implementation baseline so Technical Tasks and pull requests converge on one runtime, hosting model and dependency strategy.

The protocol and security decisions in ADR-0008, ADR-0009 and ADR-0015 remain binding. In particular, the implementation must support implicit TLS 1.2 or later, prefer TLS 1.3 and validate `SCRAM-SHA-256` without retaining a recoverable SMTP password.

.NET 10 is an LTS release and provides a cross-platform Generic Host, asynchronous sockets, pipelines, TLS, dependency injection, configuration and observability integration suitable for a long-running SMTP service. The evaluated `SmtpServer` 11.1 package advertises only `AUTH PLAIN` and `AUTH LOGIN`, so selecting that package as-is would violate Story #4 and ADR-0015.

## Decision

The first Velaris SMTP Submission implementation uses:

- .NET 10 with the `net10.0` target framework and C# 14;
- the .NET Generic Host/Worker hosting model for service lifetime, configuration, dependency injection and graceful shutdown;
- a Linux container as the production runtime target;
- PostgreSQL through Entity Framework Core 10 and the Npgsql provider;
- Entity Framework Core migrations for versioned schema evolution;
- MimeKit for defensive MIME parsing and traversal;
- OpenTelemetry for traces and metrics, with structured logging through `Microsoft.Extensions.Logging`;
- xUnit and Testcontainers for unit, integration, concurrency and failure-path tests.

Entity Framework Core is the default persistence and mapping technology. The atomic acceptance use case owns one database transaction. It may execute reviewed parameterized SQL through EF Core/Npgsql for exact conditional quota updates, conflict handling or set-based Delivery insertion when LINQ-generated statements cannot express or prove the required semantics. Such SQL remains inside the persistence adapter and is covered by PostgreSQL integration and concurrency tests.

The codebase starts as one deployable SMTP Submission service with internal boundaries for protocol, authentication, application policy, persistence, attachment storage and telemetry. It does not introduce a broker, Redis or separate deployable services for Story #4.

Package versions are pinned centrally by the repository. Compatible patch and security updates do not require a new ADR; a major-version or technology replacement requires review under RNF-MNT-006.

The SMTP engine is intentionally **not** selected by this ADR. A short Technical Spike must prove, on Linux and .NET 10:

- implicit TLS 1.2 and TLS 1.3 interoperability;
- server-side `SCRAM-SHA-256` using stored verifier material;
- no advertisement or acceptance of `PLAIN` or `LOGIN`;
- cancellation, timeouts, bounded reads and graceful connection draining;
- compatibility with the selected SMTP client test harness.

Candidates may include a maintained extensible server library, a reviewed fork or a narrowly scoped protocol implementation over the .NET TLS and I/O APIs. Forking a package or implementing protocol parsing locally requires an explicit recommendation from the Spike, including security ownership and upgrade cost.

## Consequences

- Developers have one supported runtime and project structure for Story #4.
- Entity Framework Core provides the unit of work, mapping and migration baseline while allowing explicit Npgsql/PostgreSQL statements where concurrency invariants require them.
- MimeKit avoids implementing a MIME parser while preserving the Velaris relational representation boundary from ADR-0010.
- Generic Host and OpenTelemetry use the standard .NET service lifecycle and instrumentation ecosystem.
- Work independent of the SMTP engine can begin, but the protocol/authentication task cannot move to implementation until the engine Spike concludes.
- .NET security patches, NuGet vulnerability scanning and dependency lifecycle management are part of routine maintenance.
- The Story remains implementable in .NET 10 without falling back to `PLAIN` or `LOGIN`; if the Spike cannot prove a safe SCRAM path, the conflict returns for architecture review.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- Spike #6 — Benchmark concurrent hierarchical quota acceptance
- RF-MSG-002 through RF-MSG-006
- RF-SUB-001 through RF-SUB-011
- RF-QUO-001 through RF-QUO-007
- RNF-AUT-008 and RNF-AUT-009
- RNF-DB-008 through RNF-DB-012
- RNF-MNT-001, RNF-MNT-003, RNF-MNT-004, RNF-MNT-006 and RNF-MNT-008
- RNF-OBS-001 through RNF-OBS-005 and RNF-OBS-009
- RNF-SEC-005, RNF-SEC-006, RNF-SEC-010 and RNF-SEC-011
- ADR-0008 through ADR-0013 and ADR-0015

## References

- .NET 10 lifecycle: https://learn.microsoft.com/lifecycle/products/microsoft-net-and-net-core
- .NET Worker services: https://learn.microsoft.com/dotnet/core/extensions/workers
- Entity Framework Core: https://learn.microsoft.com/ef/core/
- Npgsql Entity Framework Core provider: https://www.npgsql.org/efcore/
- MimeKit: https://github.com/jstedfast/MimeKit
- OpenTelemetry .NET: https://opentelemetry.io/docs/languages/dotnet/
