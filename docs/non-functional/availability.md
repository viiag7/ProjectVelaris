# Availability Requirements

## RNF-AVL-001 — Production Availability Objective

**Status:** PROPOSED

Before production launch, Velaris must define a measurable monthly availability objective for:

- Control Plane;
- HTTP Submission;
- SMTP Submission;
- Inbound SMTP;
- Delivery processing.

The target must reflect the system's criticality and infrastructure redundancy.

## RNF-AVL-002 — Data Plane Independence

**Status:** ACCEPTED

Temporary Control Plane unavailability must not stop processing of already accepted Messages in the Data Plane.

## RNF-AVL-003 — Worker Failure Isolation

**Status:** ACCEPTED

Failure or restart of a single worker instance must not cause loss of accepted Messages or total service outage.

## RNF-AVL-004 — No Single Worker Dependency

**Status:** ACCEPTED

Production Data Plane workloads must support multiple worker instances for each horizontally scalable critical processing function.

## RNF-AVL-005 — Maintenance Tolerance

**Status:** ACCEPTED

Routine deployment and infrastructure maintenance must be possible without requiring a complete outage of the Data Plane when the production topology is operating in high-availability mode.

## RNF-AVL-006 — Dependency Degradation

**Status:** ACCEPTED

Critical dependencies must expose health state and failure modes.

When a dependency is unavailable, services must fail predictably rather than silently corrupting, discarding or partially applying work.
