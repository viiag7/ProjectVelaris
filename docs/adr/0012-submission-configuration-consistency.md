# ADR-0012: SMTP Submission Reads Relational Configuration Directly First

**Status:** Accepted

## Context

SMTP Submission needs Credential state, Tenant and Environment state, limits, Sender Grants and Suppressions. The Data Plane must not make a synchronous Control Plane service call for every submission, but the first implementation should not introduce an event-driven configuration replica without demonstrated need.

Security-sensitive changes, especially Credential rotation or revocation, must not be weakened by stale cached data.

## Decision

The first SMTP Submission implementation reads the required configuration from its least-privilege relational source of truth through a dedicated data-access boundary. It does not depend on a synchronous Control Plane API call and does not require Redis.

Authentication resolves the Credential, Tenant, Environment and Sender Grants from that source. Before final acceptance, the service revalidates the active Credential verifier version, Credential state, Tenant state, Environment state and effective limits so a long-lived SMTP session cannot accept using authority that became stale after authentication.

If the relational source of truth is unavailable or the security context cannot be revalidated, submission fails closed with a temporary SMTP failure. Processing of already accepted Messages remains independent.

The data-access boundary must permit later replacement by a materialized read model or cache without changing SMTP protocol behavior.

Redis may be introduced only after measurements justify it. If introduced:

- the relational store remains authoritative;
- cache miss and Redis unavailability fall back to the relational source when it is healthy;
- sensitive negative or revoked state must not be hidden by stale positive cache entries;
- invalidation, TTL and the maximum revocation-propagation interval must be explicit and observable;
- a final acceptance check must preserve Tenant isolation and revocation guarantees;
- the change requires architecture review and performance evidence.

## Consequences

- The initial implementation has fewer moving parts and no cache-coherency protocol.
- Relational read capacity and connection usage must be measured and bounded.
- Temporary relational unavailability prevents new SMTP acceptance but does not stop already accepted delivery work.
- Revocation becomes effective for new authentication and acceptance checks as soon as the committed relational state is visible.
- A future cache or event-driven read model remains possible without making it an initial dependency.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- RF-CRE-003, RF-CRE-004, RF-CRE-010 and RF-CRE-014
- RF-TEN-004 and RF-TEN-015
- RF-ENV-004
- RF-SUP-003
- RF-ACL-015
- RNF-AUT-009
- RNF-ARC-002, RNF-ARC-003, RNF-ARC-004 and RNF-ARC-006
- RNF-DB-007 and RNF-DB-008
- RNF-SEC-002 through RNF-SEC-004
