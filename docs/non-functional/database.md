# Database Requirements

## RNF-DB-001 — Control and Data Persistence Separation

**Status:** ACCEPTED

Control Plane and Data Plane persistence must be logically separated.

Production architecture must permit the two planes to use separate databases and, where required by performance or recovery objectives, separate database clusters.

## RNF-DB-002 — Schema Separation

**Status:** ACCEPTED

Within a database, schemas must be used to organize bounded contexts and to support explicit ownership and permission boundaries.

## RNF-DB-003 — Schema Is Not a Performance Isolation Boundary

**Status:** ACCEPTED

Database schemas must not be treated as guaranteed CPU, memory, I/O or failure isolation boundaries.

Where independent capacity, maintenance or failure isolation is required, separate databases or clusters must be supported.

## RNF-DB-004 — Schema Is Not the Sole Backup Boundary

**Status:** ACCEPTED

Production backup and disaster-recovery design must not depend solely on schema-level logical dumps.

Recovery boundaries must be designed at database/cluster and durable-storage levels as required by the defined RPO/RTO.

## RNF-DB-005 — Bounded Context Persistence

**Status:** ACCEPTED

Persistence design must support separation of major data domains such as:

- tenancy and IAM;
- configuration;
- submission metadata;
- delivery state;
- inbound state;
- suppression;
- audit.

## RNF-DB-006 — Large Content Storage

**Status:** ACCEPTED

Large raw MIME payloads, document bodies and attachments must not be forced into the primary transactional relational database when object storage provides a more appropriate durability and scaling model.

The relational database may store metadata, integrity information and object references.

## RNF-DB-007 — Least-Privilege Database Roles

**Status:** ACCEPTED

Each application/service must use a database identity with minimum required privileges.

Application runtime roles and schema-migration roles must be separable.

## RNF-DB-008 — Connection Management

**Status:** ACCEPTED

Database connection concurrency must be bounded and observable.

Horizontal application scaling must not be allowed to exhaust database connection capacity.

## RNF-DB-009 — Transactional Consistency

**Status:** ACCEPTED

Operations that must remain consistent, including quota reservation and critical state transitions, must use transaction or concurrency controls appropriate to prevent over-allocation, lost updates and invalid intermediate states.

## RNF-DB-010 — Migration Safety

**Status:** ACCEPTED

Schema changes must be versioned, automated and designed for safe deployment in a highly available environment.

Production migrations must avoid unbounded blocking operations on high-volume tables.

## RNF-DB-011 — Data Retention

**Status:** ACCEPTED

Message content, metadata, Delivery Attempts, audit events and received documents must have explicit retention policies.

Retention periods may differ by data class and must be configurable where required by product or compliance needs.

## RNF-DB-012 — Data Integrity

**Status:** ACCEPTED

Critical records must use database constraints and integrity mechanisms where practical rather than relying exclusively on application code.
