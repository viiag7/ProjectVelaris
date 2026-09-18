# Domain Model

```mermaid
classDiagram
    Platform "1" --> "*" Tenant
    Tenant "1" --> "*" Environment
    Tenant "1" --> "*" AccessGrant
    Environment "1" --> "*" Domain
    Environment "1" --> "*" Credential
    Credential "1" --> "*" SenderGrant
    Environment "1" --> "*" Message
    Message "1" --> "*" MessageHeader
    Message "1" --> "*" MessageBodyPart
    Message "1" --> "*" Attachment
    Message "1" --> "*" Delivery
    Delivery "1" --> "*" DeliveryAttempt
    Environment "1" --> "*" Suppression
    Environment "*" --> "1" DeliveryPool
    DeliveryPool "1" --> "*" DeliveryResource
```

## Ownership

- Tenant is the primary isolation boundary.
- Environment owns message-sending configuration.
- Domain and Credential belong to an Environment.
- Sender Grants belong to a Credential.
- Message belongs to an Environment.
- MessageHeader preserves ordered, repeatable submitted headers and any normalized query projection.
- MessageBodyPart stores body content and semantic MIME metadata relationally.
- Attachment stores metadata and an opaque reference to attachment bytes in Blob/Object Storage.
- Delivery belongs to a Message and represents one recipient.
- Delivery Attempt belongs to a Delivery.
- Suppression initially belongs to an Environment.
- Delivery Pool is platform-managed and authorized to Tenants.

## Quota accounting

Usage originates from accepted recipient Deliveries at Environment level and is also aggregated into the parent Tenant quota window.

Message, its envelope, headers, bodies, attachment references, quota consumption and all recipient Deliveries share one relational acceptance transaction. Durable attachment upload precedes that transaction and is reconciled separately if the relational transaction does not commit. Messages without attachments have no Blob/Object Storage dependency.

## Authorization

Access Grants target resources and assign a role. Effective access is computed using direct grants, inherited grants and Tenant administration.
