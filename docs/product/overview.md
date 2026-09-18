# Product Overview

Project Velaris is a multi-tenant platform for outbound email submission, queueing and SMTP delivery.

The first version focuses on the delivery infrastructure layer rather than marketing automation.

## Primary capabilities

- Multi-tenant isolation.
- Multiple logical Environments per Tenant.
- HTTP API and authenticated SMTP submission.
- Sender authorization by Domain or specific Mailbox.
- Hierarchical sending quotas at Environment and Tenant levels.
- DKIM-aware Domain management.
- Asynchronous queueing and per-recipient Delivery state.
- SMTP retries based on protocol response classes.
- Delivery Pools for outbound routing.
- Message history and Delivery Attempts.
- Suppression lists.
- Resource-level access control.

## Conceptual hierarchy

```text
Platform
└── Tenant
    ├── Members
    ├── Limits / Quotas
    └── Environment
        ├── Domains
        ├── Credentials
        │   └── Sender Grants
        ├── Messages
        │   └── Deliveries
        │       └── Delivery Attempts
        └── Suppressions
```

Delivery Pools are platform-managed resources that can be authorized for Tenants and selected by Environments or routing policies.
