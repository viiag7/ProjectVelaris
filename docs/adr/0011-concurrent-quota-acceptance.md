# ADR-0011: Exact Relational Quota Counters Guard Atomic Acceptance

**Status:** Accepted

## Context

Story #4 requires one atomic relational outcome for hierarchical quota consumption, Message creation and exactly one Delivery per accepted recipient. The service runs on multiple instances and must prevent simultaneous submissions from exceeding Environment or Tenant quota.

The architecture must avoid broad pessimistic locks and process-level serialization by Tenant. It must also address hot counters, deadlocks, transaction retries and the capacity target of at least one million Deliveries per hour.

## Alternatives considered

### Broad Tenant locking

Lock a Tenant or serialize all of its submissions before checking quota.

This is simple but creates an unnecessarily large critical section and makes high-volume Tenants a submission bottleneck. It is rejected.

### Redis as the authoritative quota counter

Use atomic Redis operations before committing relational acceptance.

This reduces relational contention but creates a dual-write problem between quota consumption and Message acceptance. Compensating after partial failure would weaken exact quota enforcement. It is rejected for the initial architecture.

### Sharded or escrow quota allocation

Allocate bounded quota permits to shards or service instances and consume locally.

This can reduce hot-row contention while retaining an exact global upper bound, but it adds permit allocation, reclaim, expiry and failure-recovery complexity. It is retained as an evolution path if evidence shows exact relational counters cannot meet capacity.

### Exact relational conditional counters

Use short atomic conditional updates for the active Tenant and Environment quota windows inside the same transaction that creates Message and Deliveries.

This provides the simplest single source of truth and strongest failure semantics. It is selected for the first implementation, subject to the benchmark described below.

## Decision

Active quota usage is represented by exact relational counter rows keyed by quota scope, scope identifier, window type and window start. Uniqueness constraints prevent duplicate counters for the same window.

For a submission with `N` accepted recipients, one short relational transaction:

1. resolves the effective quota configuration/version;
2. conditionally increments the Tenant counter by `N` only when the resulting value does not exceed its limit;
3. conditionally increments the Environment counter by `N` under the same rule;
4. creates the Message, relational content and durable attachment references when present;
5. bulk-creates exactly `N` `PENDING` Deliveries;
6. commits the complete outcome.

Counter updates use one documented deterministic order across all callers. If either conditional increment affects no row because capacity is insufficient, the entire transaction rolls back. No remote service, object-store operation or cache call occurs while this transaction is open.

Database constraints, atomic data-modification statements and the narrowest isolation level proven to preserve these invariants are preferred over broad explicit locks. Deadlock or serialization conflicts use bounded retry with jitter. Exhausted retries produce a temporary SMTP failure and no accepted Message.

The implementation must expose counter contention, transaction latency, retry count, rollback reason and quota rejection metrics.

## Required validation

Before implementation of this path is considered production-ready, a time-boxed concurrency benchmark must test:

- at least the RNF-CAP-001 aggregate target plus measured headroom;
- load concentrated on one high-volume Tenant as well as distributed Tenant load;
- multiple Environments and multiple SMTP service instances;
- representative recipient counts and bulk Delivery insertion;
- quota-boundary races and proof that counters never exceed configured limits;
- deadlocks, retry rates, tail latency, database CPU/I/O and hot-row wait time;
- failure and restart behavior.

If the selected design cannot meet the capacity target with acceptable headroom, the next candidate is bounded sharded/escrow quota allocation. Replacing the exact-counter design requires a new or superseding ADR supported by benchmark evidence.

This validation is tracked by [GitHub Issue #6](https://github.com/viiag7/ProjectVelaris/issues/6), `[Spike] Benchmark concurrent hierarchical quota acceptance`.

## Consequences

- Quota and accepted Message state share one relational commit boundary.
- The initial design avoids distributed quota coordination and compensating transactions.
- Counter rows can become contention points for high-volume Tenants; this risk is measured rather than assumed away.
- Transaction bodies must remain small, deterministic and free of network I/O.
- The database remains authoritative for accepted quota consumption.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- RF-QUO-001 through RF-QUO-007
- RF-SUB-007 through RF-SUB-009
- RF-MSG-006
- RNF-CAP-001, RNF-CAP-006 and RNF-CAP-007
- RNF-DB-008, RNF-DB-009 and RNF-DB-012
- RNF-MNT-004
- ADR-0002 and ADR-0003
