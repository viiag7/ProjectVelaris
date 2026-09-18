# SMTP Submission Implementation Guidelines

This document provides initial implementation defaults for Story #4. It complements the accepted SMTP Submission ADRs without replacing requirements or turning adjustable operational parameters into permanent product rules.

The governing principle is:

> Define invariants and safe defaults now. Keep performance parameters adjustable and validate them through tests, benchmarks and observability.

## Interpretation

The terms in this document have distinct meanings:

- **Invariant:** behavior required to preserve security, isolation, integrity or the accepted architecture.
- **Initial default:** the starting implementation value or technology choice; it remains configurable or replaceable when evidence justifies a change.
- **Deferred tuning:** a parameter that must not be fixed without implementation evidence.

Requirements and accepted ADRs take precedence if a guideline conflicts with them.

## Relational persistence

PostgreSQL is the preferred relational database for the first implementation.

It stores the transactional state used by SMTP Submission, including:

- Tenant and Environment state and limits;
- Credential metadata and SCRAM verifier material;
- Sender Grants and Suppressions;
- Message, SMTP envelope, ordered headers and body parts;
- attachment metadata and opaque storage references;
- Deliveries;
- hierarchical quota counters;
- related acceptance and audit metadata.

PostgreSQL remains the authoritative source for relational acceptance and configuration reads in the initial implementation, consistent with ADR-0011 and ADR-0012.

The physical design must not prematurely fix:

- final indexes;
- table partitioning or sharding;
- database-specific tuning parameters;
- connection-pool sizes;
- batch sizes or infrastructure sizing.

Those parameters belong to Technical Tasks and must be justified by query plans, load tests, OpenTelemetry data and the Spike #6 benchmark where applicable.

## Attachment storage boundary

Only attachment bytes depend on Blob/Object Storage in the initial architecture. Message envelope, headers, bodies and attachment metadata remain relational under ADR-0010.

The domain and application layers depend on a provider-neutral attachment-storage boundary conceptually equivalent to:

```text
AttachmentStorage
  Put
  Get
  Delete
  Exists
```

Provider SDKs remain behind this boundary. A concrete adapter may later use Azure Blob Storage, OCI Object Storage, S3 or a compatible implementation without leaking provider-specific types, URLs or error models into the domain.

The relational attachment metadata supports at least:

- `AttachmentId`;
- `MessageId`;
- opaque `StorageKey` or `ObjectReference`;
- `OriginalFilename` when supplied;
- `ContentType`;
- `Size`;
- `ContentHash`;
- `HashAlgorithm`;
- `CreatedAt`;
- logical MIME position and other required MIME metadata.

Initial defaults and invariants:

- SHA-256 is the initial attachment-integrity hash algorithm.
- Object references are opaque and are not permanent public URLs.
- Object keys do not contain Tenant, sender, recipient, filename, subject or message content.
- Attachments are private by default.
- Every attachment reference committed with an accepted Message points to an object whose durable write was confirmed first.
- Hashes support integrity, correlation and diagnostics; they do not deduplicate independent SMTP submissions.

The definitive Object Storage provider remains deliberately open.

## Message representation

The first relational Message model supports at least:

```text
Message
├── SMTP Envelope
├── ordered Headers
├── text/plain Body
├── text/html Body
└── Attachments
```

Headers preserve submission order, repeated fields and submitted values. Normalized projections are added only for a demonstrated query or policy need and do not replace the preserved submitted representation.

Convenient projections for `text/plain` and `text/html` are part of the initial model. The underlying parser and body-part representation must not silently discard a valid MIME part, nesting relationship or semantic attribute merely because the convenient model does not use it.

Platform and Tenant hard limits keep relational body storage bounded. If measured body size, retention, backup or transaction cost shows that a class of body content is better suited to Object Storage under RNF-DB-006, changing that boundary requires explicit architecture review rather than an incidental schema optimization.

For valid MIME structures outside the initially supported projections, the implementation must either retain sufficient generic body-part structure and metadata or reject the submission explicitly according to the centralized SMTP response policy. Silent loss is not permitted.

An excessively broad MIME object model is not required for Story #4. Byte-for-byte raw-MIME preservation remains outside the initial architecture under ADR-0010. Exact reconstruction, forensic fidelity, DKIM-sensitive preservation or byte-identical retransmission requires separate architecture review.

## Attachment orphan reconciliation

The initial safety horizon before an unreferenced attachment object is eligible for deletion is **24 hours**.

```text
attachment uploaded
  -> relational transaction does not commit
  -> object remains unreferenced
  -> wait at least 24 hours
  -> revalidate against the relational source of truth
  -> delete only when no valid accepted Message reference exists
```

The 24-hour value is an operational default, not a permanent product rule. It is configurable and may change based on measured upload duration, replication visibility, incident recovery needs, cost and reconciliation behavior.

The reconciler is:

- idempotent and safe under concurrent executions;
- retryable with observable outcomes;
- restricted to non-public attachment storage;
- required to revalidate relational references immediately before deletion;
- incapable by design of deleting an attachment referenced by an accepted Message;
- instrumented for candidates, deletions, skips, failures, retries and oldest-orphan age.

## Relational transaction retry

The initial maximum is **3 total transaction attempts**, including the first attempt. The value remains configurable.

Retry applies only to failures explicitly classified as transient and safe to repeat, such as:

- deadlock victims;
- serialization conflicts;
- other database conflict conditions deliberately classified as retryable.

The implementation does not retry indiscriminately on authentication failures, constraint violations, quota exhaustion, invalid data, programming errors or unknown database errors.

Retries use a small backoff with jitter, but exact delay values and the jitter algorithm remain deferred until evidence exists. Each failed attempt rolls back completely. After the final attempt:

- no Message is accepted;
- no partial Delivery set remains;
- quota consumption is not committed;
- the centralized SMTP response policy returns an appropriate temporary `4yz` failure.

Attachment uploads are not repeated inside the relational retry loop. The relational transaction contains no Object Storage or other remote call.

## Transaction isolation and quota validation

No transaction isolation level is selected by this guideline.

Spike #6 evaluates `READ COMMITTED`, `REPEATABLE READ`, `SERIALIZABLE` and applicable PostgreSQL mechanisms against the exact conditional-counter strategy in ADR-0011. The selected approach is the simplest and most performant one that proves all invariants:

- Tenant quota never exceeds its configured limit;
- Environment quota never exceeds its configured limit;
- Message, attachment references, quota consumption and all Deliveries commit or roll back together;
- no failure exposes partial acceptance;
- multiple SMTP instances remain concurrency-safe.

Correctness and performance are evaluated together. Isolation is not strengthened merely as a substitute for understanding the actual statements and constraints.

## Configuration lookup

The initial lookup path remains:

```text
SMTP Submission -> relational source of truth
```

Redis is not an initial dependency. PostgreSQL is authoritative for Credential state and verifier version, Tenant and Environment state, limits, Sender Grants and Suppressions.

If measurements later show relational reads are a material bottleneck, introducing Redis or another cache requires review of:

- TTL and invalidation;
- Credential rotation and revocation;
- fail-closed behavior;
- Tenant isolation;
- cache-miss fallback;
- cache unavailability;
- measurable propagation delay.

Caching must not weaken the final security-sensitive revalidation required by ADR-0012.

## Identifiers

UUID is the initial default for distributed Velaris identifiers when no requirement specifies otherwise.

Business behavior must not depend on natural UUID ordering. The exact UUID version and physical index strategy remain schema-design decisions. UUIDv7 or another order-friendly strategy may be adopted later if database evidence demonstrates a useful indexing or locality benefit.

## Date and time

Internal timestamps are persisted and processed in UTC. Internal rules must not depend on the local timezone of a host or container.

Timezone conversion belongs at presentation or API boundaries when required. Tests use explicit clocks where time controls quota windows, expiration, session duration or orphan eligibility.

## Centralized SMTP response policy

Internal failures are translated to SMTP replies through one centralized policy. Protocol codes and Enhanced Status Codes are not hard-coded throughout handlers or infrastructure adapters.

The initial semantic classification is:

| Internal condition | Classification |
|---|---|
| Invalid Credential | Permanent / `5yz` |
| Revoked Credential | Permanent / `5yz` |
| Unauthorized sender | Permanent / `5yz` |
| Deterministic configured-limit violation | Permanent / `5yz` |
| Suppressed recipient | Permanent / `5yz` |
| Relational database unavailable | Temporary / `4yz` |
| Attachment storage unavailable | Temporary / `4yz` |
| Transaction retries exhausted | Temporary / `4yz` |
| Temporary resource condition | Temporary / `4yz` |

The Technical Task for SMTP response mapping selects exact SMTP and Enhanced Status Codes from the applicable RFCs and adds protocol-level tests. This guideline does not invent numeric replies merely to close documentation.

## At-least-once submission

ADR-0013 remains authoritative.

Client `Message-ID`, attachment hash, body hash and aggregate content hash may support correlation, diagnostics, observability and investigation. They must not automatically deduplicate independent SMTP submissions.

An internal retry of the same Velaris operation is idempotent where applicable. A new client submission after an ambiguous final SMTP response may legitimately create a new Message and Delivery set.

## Defensive platform limits

The implementation supports a platform hard ceiling above each configurable Tenant limit:

```text
effective submission limit = min(platform hard limit, Tenant configured limit)
```

A Tenant cannot configure or exercise a value above the platform ceiling. Defensive hard limits exist for at least:

- total message size;
- recipients per submission;
- attachment count;
- individual attachment size;
- header count;
- individual and aggregate header size;
- MIME nesting depth;
- SMTP session duration.

These ceilings protect parser depth, memory, CPU, storage, transaction size and predictable capacity. Their numeric defaults remain open for a Technical Task and security review; this document defines the mechanism, not arbitrary values.

## Observability

OpenTelemetry is the initial standard for distributed traces, metrics and structured-log correlation.

Telemetry should correlate, using safe identifiers:

```text
SMTP Session
  -> Authentication
  -> Credential
  -> Tenant / Environment
  -> MAIL FROM / recipients
  -> Attachment Storage
  -> Quota
  -> Relational Transaction
  -> Message / Deliveries
  -> SMTP response
```

It exposes enough information to analyze:

- authentication and parsing latency;
- attachment count, bytes, upload latency and failures;
- database and acceptance-transaction latency;
- quota contention and conditional-update failures;
- transaction attempts, retry causes and exhausted retries;
- total SMTP acceptance latency and rejection reasons;
- orphan candidates, reconciliation age, deletions and failures.

Logs, spans and metric attributes must not contain Credential secrets, SCRAM verifier material, message bodies, attachment content or unnecessary sensitive personal data. High-cardinality identifiers are used deliberately and according to the telemetry backend's capacity and access controls.

## Deliberately deferred tuning

The following are not fixed without implementation evidence:

- final transaction isolation level;
- PostgreSQL connection-pool size;
- worker and SMTP instance counts;
- Delivery insertion batch size;
- table partitioning and sharding;
- Redis, Redis TTL or invalidation implementation;
- physical indexes;
- PostgreSQL tuning parameters;
- exact retry delays and jitter algorithm;
- sharded or escrow quota allocation;
- infrastructure sizing.

The expected feedback loop is:

```text
requirements
  -> implementation
  -> correctness and failure tests
  -> OpenTelemetry
  -> benchmark
  -> evidence
  -> tuning
```

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- Spike #6 — Benchmark concurrent hierarchical quota acceptance
- RF-SUB-001 through RF-SUB-011
- RF-CRE-003, RF-CRE-012 and RF-CRE-014
- RF-MSG-005 and RF-MSG-006
- RF-QUO-001 through RF-QUO-007
- RNF-AUT-008 and RNF-AUT-009
- RNF-CAP-001, RNF-CAP-006 and RNF-CAP-007
- RNF-DB-006, RNF-DB-008 through RNF-DB-012
- RNF-MNT-004 and RNF-MNT-005
- RNF-OBS-001 through RNF-OBS-005 and RNF-OBS-009
- ADR-0008 through ADR-0013
