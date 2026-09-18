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
    SUP -- no --> EQ{Environment quota available?}
    EQ -- no --> R5[Reject quota]
    EQ -- yes --> TQ{Tenant quota available?}
    TQ -- no --> R6[Reject quota]
    TQ -- yes --> ACCEPT[Accept and persist]
    ACCEPT --> DEL[Create Delivery per recipient]
    DEL --> QUEUE[Queue]
```

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
