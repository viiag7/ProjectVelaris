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

## ADR template

Each ADR should contain:

- Status
- Context
- Decision
- Consequences

Accepted ADRs should not be silently rewritten when a decision changes. Prefer superseding them with a new ADR.
