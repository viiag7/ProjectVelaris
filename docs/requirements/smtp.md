# Outbound SMTP Delivery Requirements

This document governs Velaris-to-recipient-system SMTP delivery. Authenticated client-to-Velaris submission is governed separately by [SMTP Submission Requirements](smtp-submission.md).

Outbound SMTP delivery behavior must follow the semantics of SMTP reply classes and preserve enough protocol detail for reliable retries and troubleshooting.

## RF-SMTP-001 — 2yz Replies

An applicable SMTP reply in the `2yz` class must be treated as successful completion of the corresponding operation.

## RF-SMTP-002 — 3yz Replies

Replies in the `3yz` class are intermediate protocol states and must not be interpreted as a final Delivery outcome.

## RF-SMTP-003 — 4yz Replies

An SMTP reply in the `4yz` class must be treated as a temporary failure.

The affected Delivery must remain eligible for retry.

## RF-SMTP-004 — 5yz Replies

An SMTP reply in the `5yz` class must be treated as a permanent failure for the affected command or recipient.

The system must not repeatedly retry the same operation as though the condition were transient.

## RF-SMTP-005 — Per-RCPT Result

Responses to individual `RCPT TO` commands must be evaluated independently.

Failure for one recipient must not automatically determine the outcome of other recipients.

## RF-SMTP-006 — Enhanced Status Codes

When supplied by the remote server, the Enhanced Mail System Status Code must be stored separately from the three-digit SMTP reply code.

Example:

```text
smtp_code:       550
enhanced_status: 5.1.1
response:        User unknown
```

## RF-SMTP-007 — Delivery Attempt Data

Each Delivery Attempt must record, when available:

- timestamp;
- recipient;
- destination MX;
- remote IP address;
- selected Delivery Pool;
- selected outbound resource;
- SMTP session stage;
- SMTP reply code;
- Enhanced Status Code;
- remote response text;
- attempt duration.

## RF-SMTP-008 — Retry

Temporary failures must schedule a new Delivery Attempt according to the active retry policy.

## RF-SMTP-009 — Retry Policy

Retry policy must be configurable and aligned with SMTP protocol guidance.

## RF-SMTP-010 — Expiration

A Delivery must not remain indefinitely eligible for retry.

After the configured maximum retry period it must transition to `EXPIRED`.

## References

- RFC 5321 — Simple Mail Transfer Protocol: https://www.rfc-editor.org/rfc/rfc5321
- RFC 3463 — Enhanced Mail System Status Codes: https://www.rfc-editor.org/rfc/rfc3463
