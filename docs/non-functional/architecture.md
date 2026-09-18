# Architecture Boundary Requirements

## RNF-ARC-001 — Control Plane / Data Plane Separation

**Status:** ACCEPTED

Velaris must separate the Control Plane from the Data Plane as distinct architectural boundaries.

The Control Plane manages configuration, identity, authorization, Tenants, Environments, Domains, Credentials, quotas and delivery configuration.

The Data Plane performs submission, inbound reception, queue processing, outbound delivery and document-processing workloads.

## RNF-ARC-002 — No Per-Delivery Control Plane Dependency

**Status:** ACCEPTED

The Data Plane must not require a synchronous Control Plane request for every Message, Delivery or Delivery Attempt.

## RNF-ARC-003 — Configuration Distribution

**Status:** ACCEPTED

Configuration required by the Data Plane must be distributable, cacheable or materialized so that the Data Plane can continue operating during temporary Control Plane unavailability.

## RNF-ARC-004 — Failure Isolation

**Status:** ACCEPTED

Failure in the Control Plane must be isolated from the core outbound and inbound processing paths as far as operationally possible.

## RNF-ARC-005 — Security Boundary

**Status:** ACCEPTED

Control Plane and Data Plane components must use distinct service identities, permissions and network policies appropriate to their responsibilities.

## RNF-ARC-006 — Durable State Ownership

**Status:** ACCEPTED

Each critical state transition must have a clearly defined durable source of truth.

Critical processing must not depend on in-memory state of a single service instance.
