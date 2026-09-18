# ADR-0006: SMTP Reply Semantics Drive Delivery Behavior

**Status:** Accepted

## Context

Remote SMTP servers return structured reply codes. Free-form text varies widely and should not be the primary signal for deciding retry behavior.

## Decision

Velaris uses SMTP reply classes as the primary protocol signal:

- `2yz`: positive completion;
- `3yz`: intermediate state;
- `4yz`: temporary failure, eligible for retry;
- `5yz`: permanent failure for the affected operation/recipient.

Enhanced Status Codes are stored separately when available.

## Consequences

- Retry behavior follows protocol semantics.
- Response text remains available for troubleshooting and classification.
- Per-`RCPT TO` outcomes are stored independently.
- Retry expiration must be configurable.

## References

- https://www.rfc-editor.org/rfc/rfc5321
- https://www.rfc-editor.org/rfc/rfc3463
