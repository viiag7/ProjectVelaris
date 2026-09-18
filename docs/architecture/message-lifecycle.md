# Message Lifecycle

## Submission lifecycle

```mermaid
flowchart TD
    S[Submission] --> AUTH{Credential valid?}
    AUTH -- no --> R1[Reject]
    AUTH -- yes --> SG{Sender Grant valid?}
    SG -- no --> R2[Reject]
    SG -- yes --> ST{Tenant and Environment active?}
    ST -- no --> R3[Reject temporarily]
    ST -- yes --> SUP{Recipient suppressed?}
    SUP -- yes --> R4[Reject / suppress recipient]
    SUP -- no --> EQ{Environment quota precheck?}
    EQ -- no --> R5[Reject quota]
    EQ -- yes --> TQ{Tenant quota precheck?}
    TQ -- no --> R6[Reject quota]
    TQ -- yes --> HAS{Attachments?}
    HAS -- yes --> OBJ[Durably store every attachment]
    HAS -- no --> ACCEPT[Relational acceptance transaction]
    OBJ -- failure --> R7[Reject]
    OBJ -- success --> ACCEPT
    ACCEPT -- failure --> R8[Rollback and reject temporarily]
    ACCEPT --> ACK[Return final submission success]
    ACK --> STOP[[First-increment boundary]]
    STOP -. later processing .-> QUEUE[Queue processing]
```

The acceptance transaction stores the envelope, headers, body, metadata and attachment references and creates exactly one unqueued Delivery for every accepted envelope recipient. An attachment uploaded before a failed relational transaction is an unreachable orphan handled by safe reconciliation. Messages without attachments use no Object Storage operation. Queue processing begins only after the successful SMTP submission boundary.

If relational commit succeeds but the client does not observe the final SMTP reply, the accepted Message remains valid. A retransmission is a separate submission and may create another Message under the at-least-once semantics in ADR-0013.

## Delivery lifecycle

```mermaid
stateDiagram-v2
    [*] --> PENDING
    PENDING --> QUEUED
    QUEUED --> PROCESSING
    PROCESSING --> DELIVERED: successful SMTP completion
    PROCESSING --> DEFERRED: temporary failure
    PROCESSING --> FAILED: permanent failure
    DEFERRED --> QUEUED: retry scheduled
    DEFERRED --> EXPIRED: retry window exhausted
    PENDING --> CANCELLED
    QUEUED --> CANCELLED
```

A Message may contain Deliveries in different states simultaneously. Message-level state is therefore an aggregate view rather than the source of truth for recipient delivery outcome.
