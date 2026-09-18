# Backup and Disaster Recovery Requirements

## RNF-DR-001 — Automated Backups

**Status:** ACCEPTED

Critical persistent data stores must have automated backups appropriate to their data type and recovery objective.

## RNF-DR-002 — Point-in-Time Recovery

**Status:** ACCEPTED

Transactional databases containing critical state must support point-in-time recovery or an equivalent mechanism capable of restoring to a bounded point before a failure.

## RNF-DR-003 — Backup Encryption

**Status:** ACCEPTED

Backups containing sensitive or customer data must be encrypted at rest and in transit.

## RNF-DR-004 — Backup Isolation

**Status:** ACCEPTED

Backup data must be isolated so compromise or deletion of a production workload does not automatically imply loss of all recoverable copies.

## RNF-DR-005 — Restore Testing

**Status:** ACCEPTED

Backup success alone is insufficient.

Restore procedures must be exercised periodically and results must be recorded.

## RNF-DR-006 — RPO

**Status:** PROPOSED

A formal Recovery Point Objective must be defined for each critical persistence domain before production readiness approval.

Control Plane, delivery state, inbound documents and audit data may have different RPOs.

## RNF-DR-007 — RTO

**Status:** PROPOSED

A formal Recovery Time Objective must be defined for each critical service/data domain before production readiness approval.

## RNF-DR-008 — Document/Object Recovery

**Status:** ACCEPTED

Object storage containing MIME content, attachments and fiscal documents must have a documented durability, versioning/retention and recovery strategy.

## RNF-DR-009 — Recovery Dependencies

**Status:** ACCEPTED

Disaster recovery plans must include all dependencies required to resume processing, including configuration, cryptographic material, DNS-related configuration and delivery infrastructure metadata.

## RNF-DR-010 — Recovery Runbook

**Status:** ACCEPTED

Production readiness requires a documented and testable disaster-recovery runbook.
