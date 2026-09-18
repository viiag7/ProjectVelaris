# ADR-0002: One Delivery per Recipient

**Status:** Accepted

## Context

Recipients of the same Message may receive different SMTP outcomes.

Example:

```text
recipient A -> 250
recipient B -> 451
recipient C -> 550
```

A single Message status cannot accurately represent those independent outcomes.

## Decision

Every recipient creates an independent Delivery.

```text
Message
├── Delivery
├── Delivery
└── Delivery
```

Delivery is the source of truth for recipient outcome.

## Consequences

This enables:

- independent retry;
- independent SMTP status;
- accurate quota accounting;
- suppression by recipient;
- per-recipient observability;
- aggregate Message state without losing detail.
