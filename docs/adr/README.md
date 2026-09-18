# Architecture Decision Records

ADRs document decisions that materially shape Velaris.

| ADR | Decision | Status |
|---|---|---|
| [0001](0001-tenant-environment-model.md) | Tenant → Environment tenancy model | Accepted |
| [0002](0002-delivery-per-recipient.md) | One Delivery per recipient | Accepted |
| [0003](0003-hierarchical-quotas.md) | Environment + Tenant hierarchical quotas | Accepted |
| [0004](0004-credential-sender-grants.md) | Credentials use Sender Grants | Accepted |
| [0005](0005-delivery-pools.md) | Delivery Pool abstraction | Accepted |
| [0006](0006-smtp-reply-semantics.md) | SMTP reply classes drive delivery behavior | Accepted |
| [0007](0007-resource-access-control.md) | Resource-level Reader/Editor/Admin authorization | Accepted |
| [0008](0008-smtp-tls-termination.md) | SMTP TLS terminates in the Submission service | Accepted; partially superseded by ADR-0015 |
| [0009](0009-smtp-scram-credential-verifiers.md) | SMTP Credentials store SCRAM verifiers | Accepted; amended by ADR-0015 |
| [0010](0010-message-and-attachment-persistence.md) | Message data is relational and attachments use Object Storage | Accepted |
| [0011](0011-concurrent-quota-acceptance.md) | Exact relational quota counters guard atomic acceptance | Accepted |
| [0012](0012-submission-configuration-consistency.md) | Submission reads relational configuration directly first | Accepted |
| [0013](0013-smtp-at-least-once-submission.md) | SMTP submission has at-least-once acceptance semantics | Accepted |
| [0014](0014-dotnet-10-submission-stack.md) | SMTP Submission uses .NET 10 | Accepted |
| [0015](0015-scram-sha-256-over-implicit-tls.md) | SMTP authentication uses SCRAM-SHA-256 over implicit TLS | Accepted |

## ADR template

Each ADR should contain:

- Status
- Context
- Decision
- Consequences

Accepted ADRs should not be silently rewritten when a decision changes. Prefer superseding them with a new ADR.
