# Non-Functional Requirements

This directory defines the operational, security, scalability, resilience and maintainability requirements for Velaris.

These requirements describe properties the platform **must guarantee**, independently from most implementation choices.

## Requirement areas

| Area | Prefix | Document |
|---|---|---|
| Capacity | RNF-CAP | [capacity.md](capacity.md) |
| Availability | RNF-AVL | [availability.md](availability.md) |
| Scalability | RNF-SCA | [scalability.md](scalability.md) |
| Architecture Boundaries | RNF-ARC | [architecture.md](architecture.md) |
| Security | RNF-SEC | [security.md](security.md) |
| Authentication | RNF-AUT | [authentication.md](authentication.md) |
| Authorization | RNF-AUZ | [authorization.md](authorization.md) |
| Database | RNF-DB | [database.md](database.md) |
| Messaging | RNF-MQ | [messaging.md](messaging.md) |
| Containers | RNF-CTR | [containers.md](containers.md) |
| Observability | RNF-OBS | [observability.md](observability.md) |
| Backup & Disaster Recovery | RNF-DR | [backup-disaster-recovery.md](backup-disaster-recovery.md) |
| Deployment & IaC | RNF-DEP | [deployment.md](deployment.md) |
| Inbound Processing Security | RNF-INB | [inbound-security.md](inbound-security.md) |
| Maintainability | RNF-MNT | [maintainability.md](maintainability.md) |

## Status

A requirement may be:

- `ACCEPTED`: agreed product/platform requirement.
- `PROPOSED`: target still requires benchmark, cost or operational validation.
- `SUPERSEDED`: replaced by another requirement.

## Verification

Each requirement should be verifiable through one or more of:

- automated tests;
- load tests;
- security tests;
- chaos/failure tests;
- architecture review;
- infrastructure policy;
- operational monitoring;
- backup/restore exercises.

Functional requirements define **what Velaris does**. Non-functional requirements define **how reliably, securely and at what scale Velaris must do it**.
