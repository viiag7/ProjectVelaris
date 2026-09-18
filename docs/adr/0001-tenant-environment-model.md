# ADR-0001: Tenant → Environment Model

**Status:** Accepted

## Context

Velaris requires multi-tenant isolation and multiple logical sending contexts per customer. The word “server” can be confused with a physical host or MTA process.

## Decision

The primary ownership model is:

```text
Platform
└── Tenant
    └── Environment
```

A Tenant is the customer/isolation boundary. An Environment is a logical sending context and does not imply a physical server.

## Consequences

- Limits and permissions can be applied at Tenant and Environment levels.
- A Tenant may isolate production, staging, billing or authentication traffic.
- Delivery infrastructure may scale independently from the logical Environment model.
