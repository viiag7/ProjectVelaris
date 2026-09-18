# Architecture Overview

This document captures the current logical architecture. It is intentionally technology-neutral while requirements are still evolving.

## Core flow

```mermaid
flowchart TD
    C[Client] --> A[HTTP API / SMTP Submission]
    A --> AUTH[Credential Authentication]
    AUTH --> SG[Sender Grant Validation]
    SG --> STATUS[Tenant + Environment State]
    STATUS --> SUP[Suppression Check]
    SUP --> EQ[Environment Quota]
    EQ --> TQ[Tenant Aggregate Quota]
    TQ --> M[Atomically persist Message and one Delivery per recipient]
    M --> ACK[Return submission success]
    ACK --> BOUNDARY[[Acceptance boundary]]
    BOUNDARY --> Q[Queue processing]
    Q --> DP[Select Delivery Pool]
    DP --> MX[Resolve MX]
    MX --> SMTP[SMTP Delivery]
    SMTP --> ATT[Delivery Attempt]
```

The first increment ends at the acceptance boundary. Final SMTP submission success depends on atomic persistence of the Message, envelope, content, audit metadata and one unqueued Delivery per accepted recipient, but not on queue processing or outbound delivery.

## Main boundaries

### Control plane

Responsible for Tenant, Environment, Domain, Credential, ACL, quota configuration and Delivery Pool configuration.

### Submission plane

Accepts authenticated HTTP API and SMTP submissions, performs required policy validation and safely persists accepted submission data.

### Delivery plane

Processes Deliveries asynchronously, applies routing, resolves destination MX hosts, performs SMTP delivery and schedules retries.

### Observability plane

Provides message history, Delivery Attempts, usage metrics and dashboards.

## Design principles

- Tenant isolation is mandatory.
- Delivery outcome is per recipient.
- Quotas are hierarchical.
- Authentication and sender authorization are separate concerns.
- Delivery Pools abstract outbound routing resources.
- Protocol replies, not free-form response text, drive SMTP retry semantics.
