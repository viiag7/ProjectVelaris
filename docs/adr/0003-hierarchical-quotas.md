# ADR-0003: Hierarchical Sending Quotas

**Status:** Accepted

## Context

A Tenant may contain several Environments. The platform needs to constrain both an individual Environment and the Tenant as a whole.

## Decision

Sending quota is enforced hierarchically.

```text
Environment quota
      ↓
Tenant aggregate quota
```

Every accepted recipient consumes one quota unit.

A submission is accepted only when both the Environment and Tenant have sufficient capacity.

## Consequences

- One Environment cannot exceed its own allocation.
- Multiple Environments cannot collectively exceed the Tenant allocation.
- Tenant usage is an aggregate of Environment usage.
- Quota consumption must be concurrency-safe.
