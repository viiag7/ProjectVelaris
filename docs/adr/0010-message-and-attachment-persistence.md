# ADR-0010: Message Data Is Relational and Attachments Use Object Storage

**Status:** Accepted

## Context

Story #4 requires Velaris to preserve the SMTP envelope and submitted message content, consume quota and create one Delivery per accepted recipient before returning SMTP success.

Attachments can be large and are not a good default fit for the primary transactional database. However, making every Message depend on Blob/Object Storage is unnecessary when a message has no attachments. Blob/Object Storage and the relational database do not share one ACID transaction, and Velaris must not introduce a distributed transaction between them.

## Alternatives considered

### Complete raw MIME in Blob/Object Storage

Storing the complete submitted byte stream provides the strongest byte-level fidelity for auditing, exact reconstruction, later retransmission and investigation of MIME parsing differences. It may also preserve inputs relevant to future signing or canonicalization analysis.

It makes every submission depend on Object Storage, duplicates data if parsed bodies and attachments are also retained, complicates retention and can preserve malformed or undesirable encodings that the product does not intend to reproduce verbatim.

This option is not selected for the initial architecture. Adding raw-MIME archival requires explicit product and architecture review covering fidelity requirements, DKIM ownership, retention, privacy, cost and whether exact byte reproduction is a supported product behavior.

### Complete raw MIME in the relational database

This offers transactional coupling with Message state and byte-level fidelity, but places large opaque payloads and attachments in the primary relational store. It conflicts with the intended database scaling and backup profile and is rejected.

### Relational Message content with attachment objects

Store the envelope, ordered headers, body parts and message metadata relationally while storing attachment bytes in durable Object Storage. Messages without attachments use only the relational acceptance path.

This keeps searchable and operational Message data transactional while isolating large binary content. It is selected.

## Decision

The relational acceptance model stores:

- SMTP envelope independently from message headers;
- ordered, repeatable headers with their submitted values plus any normalized projection required for querying or policy;
- body parts, including content, media type, charset, transfer-encoding metadata, content identifier, disposition and logical MIME position where applicable;
- submission and audit metadata;
- exactly one Delivery per accepted envelope recipient;
- attachment metadata and opaque object references.

The initial model preserves message semantics but does not promise byte-for-byte reconstruction of the original raw MIME stream. The exact physical schema remains an implementation design decision subject to the relational storage, migration and capacity RNFs.

Attachment bytes are stored in durable Blob/Object Storage. For each attachment, the relational model retains at least:

- attachment identifier;
- owning Message identifier;
- opaque object reference;
- original filename when supplied;
- content type;
- content size;
- content hash/checksum and algorithm;
- object-storage version or integrity token when provided;
- logical MIME position and other metadata needed to relate the attachment to the submitted content.

For a Message containing attachments, SMTP acceptance follows this order:

1. parse and validate the bounded submitted message while streaming each attachment to a tenant-scoped, non-public object key;
2. obtain durable-write confirmation for every attachment and validate its observed size and integrity metadata;
3. execute the relational acceptance transaction that consumes quota and creates the Message, envelope, headers, bodies, attachment references and exactly one `PENDING` Delivery per accepted recipient;
4. return the final SMTP `250` only after the relational transaction commits.

A Message without attachments skips Blob/Object Storage and follows the relational acceptance path directly.

The relational database is the source of truth for whether a Message is accepted. An uploaded attachment without a committed Message reference is an orphan, not accepted Message content.

No distributed transaction is used. An idempotent reconciliation process identifies unreferenced attachment objects older than a safety horizon and deletes them. It must never delete an object referenced by an accepted Message. A separate integrity check detects missing or corrupt referenced attachments and raises an operationally critical condition.

## Failure behavior

| Failure point | Result |
|---|---|
| MIME parsing, validation or any attachment upload fails | Return an appropriate SMTP failure; create no Message or Delivery. Already uploaded attachment objects remain unreachable and eligible for reconciliation. |
| All attachments are durable but the relational transaction fails | Return a temporary SMTP failure; leave unreachable orphans eligible for reconciliation. |
| Relational commit succeeds but the connection fails before `250` is observed | Keep the accepted Message and Deliveries; a client retry is a separate submission under ADR-0013. |
| A committed attachment reference later resolves to missing or corrupt content | Do not process the affected Message; surface an integrity incident for reconciliation. |

## Consequences

- Messages without attachments have no Object Storage dependency.
- Headers, bodies, envelope, metadata and Delivery creation share the relational acceptance transaction.
- Large attachment bytes do not inflate the primary transactional database by default.
- Final SMTP success waits for durable storage of every attachment and the relational commit.
- Attachment object lifecycle, encryption, retention, backup and reconciliation become production dependencies.
- Orphan attachment objects are expected failure residue and must remain inaccessible until safely removed.
- Object keys must not disclose Tenant, sender, recipient, filename or message content.
- Content hashes provide integrity evidence but are not submission-deduplication keys.
- Relational body size, retention, backup growth and transaction cost must be included in capacity validation using Tenant-configured message limits.
- Exact raw-MIME archival remains intentionally undecided and must not be introduced as an incidental implementation choice.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- RF-SUB-006, RF-SUB-008 and RF-SUB-009
- RF-MSG-003 through RF-MSG-006
- RNF-DB-006, RNF-DB-009, RNF-DB-011 and RNF-DB-012
- RNF-DR-001, RNF-DR-003 and RNF-DR-008
- RNF-SEC-004 through RNF-SEC-007
