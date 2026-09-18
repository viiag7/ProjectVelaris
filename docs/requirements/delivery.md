# Delivery Requirements

## RF-DEL-001 — Independent Recipient Delivery

Each recipient must have an independent Delivery.

Example:

```text
Message msg_123
├── user-a@example.com -> DELIVERED
├── user-b@example.com -> DEFERRED
└── user-c@example.com -> FAILED
```

## RF-DEL-002 — Delivery States

The initial Delivery state model must support at least:

- `PENDING`
- `QUEUED`
- `PROCESSING`
- `DEFERRED`
- `DELIVERED`
- `FAILED`
- `EXPIRED`
- `CANCELLED`

## RF-DEL-003 — Delivery Attempts

A Delivery may contain multiple Delivery Attempts.

Each attempt records one outbound delivery interaction and its result.
