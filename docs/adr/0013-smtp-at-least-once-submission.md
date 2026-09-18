# ADR-0013: SMTP Submission Has At-Least-Once Acceptance Semantics

**Status:** Accepted

## Context

After Velaris commits an accepted Message and its Deliveries, the connection can fail before the client observes the final SMTP `250`. SMTP permits the client to retransmit because it cannot know whether the server accepted responsibility.

Headers such as `Message-ID` are client-controlled, may be absent or repeated intentionally and are not safe deduplication keys.

## Decision

SMTP submission has at-least-once acceptance semantics across ambiguous connection failure.

Once relational acceptance commits, the Message and Deliveries remain accepted even if the final `250` cannot be delivered. A later client retransmission is a new SMTP submission and may create a new Velaris Message and a new set of Deliveries.

Velaris does not suppress a submission solely because another Message has the same client `Message-ID`, content hash, sender, recipients or nearby timestamp.

Each accepted submission receives its own Velaris Message identifier. For diagnosis and customer support, Velaris retains safe correlation metadata such as client `Message-ID` when present, content hash, Credential, connection/session correlation identifier and acceptance timestamp. Correlation metadata does not change acceptance identity.

Internal asynchronous components remain idempotent by Velaris Message, Delivery and work-item identifiers. Preventing a duplicate side effect while retrying the same internal work item is distinct from deduplicating two independent SMTP transactions.

## Consequences

- Ambiguous failures can produce duplicate accepted Messages and downstream deliveries.
- Velaris does not risk dropping a legitimate submission through heuristic deduplication.
- Operational tooling can correlate likely retransmissions without declaring them identical automatically.
- Clients that require request-level idempotency need a future protocol/API capability designed with an explicit idempotency key; SMTP `Message-ID` is insufficient.
- Tests must cover connection loss after commit and before the final reply.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- RF-SUB-009 through RF-SUB-011
- RF-MSG-005 and RF-MSG-006
- RNF-MQ-005
- RNF-OBS-002, RNF-OBS-004 and RNF-OBS-005
- RNF-MNT-004
