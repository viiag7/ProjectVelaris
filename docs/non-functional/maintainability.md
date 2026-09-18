# Maintainability Requirements

## RNF-MNT-001 — Versioned Database Migrations

**Status:** ACCEPTED

Database schema changes must be represented by versioned migrations tracked with source code.

## RNF-MNT-002 — Public API Versioning

**Status:** ACCEPTED

Externally consumed APIs must have a documented compatibility and versioning policy.

## RNF-MNT-003 — Configuration Externalization

**Status:** ACCEPTED

Environment-specific configuration must not require recompiling the application.

## RNF-MNT-004 — Automated Tests

**Status:** ACCEPTED

Critical behavior must be covered by automated tests at appropriate levels, including authorization, Tenant isolation, quota concurrency and delivery state transitions.

## RNF-MNT-005 — Backward-Compatible Rolling Changes

**Status:** ACCEPTED

Where practical, service and schema changes must support rolling deployment without requiring simultaneous replacement of every instance.

## RNF-MNT-006 — Dependency Lifecycle

**Status:** ACCEPTED

Critical third-party dependencies must have an explicit upgrade and vulnerability-management process.

## RNF-MNT-007 — Operational Documentation

**Status:** ACCEPTED

Critical subsystems must have sufficient operational documentation for deployment, troubleshooting and recovery.

## RNF-MNT-008 — Architecture Decision Traceability

**Status:** ACCEPTED

Implementation choices that materially affect non-functional requirements must be traceable to architecture decisions without rewriting the requirements themselves.
