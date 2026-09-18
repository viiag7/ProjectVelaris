# SMTP Submission Requirements

SMTP submission is the authenticated client-to-Velaris protocol boundary. It is distinct from outbound SMTP delivery to recipient mail systems.

## RF-SUB-001 — Authentication Required

Velaris must require successful SMTP authentication before accepting a Message.

An unauthenticated transaction must be rejected and must not create an accepted Message.

The initial SMTP submission service must:

- use implicit TLS;
- prefer TLS 1.3 and require TLS 1.2 or later;
- disable TLS 1.0, TLS 1.1 and plaintext submission;
- support `SCRAM-SHA-256-PLUS` with channel binding as its authentication mechanism;
- reject `PLAIN`, `LOGIN` and authentication outside an encrypted session.

## RF-SUB-002 — Credential-Derived Context

Successful SMTP authentication must resolve exactly one Credential, Environment and Tenant.

The client must not be able to replace that context through SMTP envelope values, message headers or message content.

## RF-SUB-003 — Resource Eligibility

Velaris must reject submission when the Credential is revoked or its Tenant or Environment is not active.

SMTP submission failures must be classified as:

- **soft bounce**: a `4yz` temporary failure that the client may retry;
- **hard bounce**: a `5yz` permanent failure that requires the request, Credential or policy condition to change before retry.

Transient platform or persistence failures, temporary resource state and temporarily unavailable quota must produce a soft bounce. Invalid or revoked Credentials, unauthorized senders, Suppressions and deterministic policy or configured-limit violations must produce a hard bounce.

## RF-SUB-004 — Envelope Sender Authorization

Velaris must validate the `MAIL FROM` identity against the authenticating Credential's Sender Grants before accepting message content.

An unauthorized sender must be rejected without creating an accepted Message.

## RF-SUB-005 — Envelope Recipients

Velaris must evaluate each `RCPT TO` command according to submission policy and retain every accepted envelope recipient.

At least one envelope recipient must be accepted before Velaris accepts `DATA`.

Velaris must reject recipients beyond the Tenant's configured per-submission recipient limit.

## RF-SUB-006 — Submitted Content

After at least one recipient is accepted, Velaris must process a valid `DATA` transaction and preserve the submitted message content separately from SMTP envelope metadata.

Velaris must enforce the Tenant's configured maximum message size and maximum SMTP session duration.

## RF-SUB-007 — Pre-Acceptance Policy

Before accepting a Message, Velaris must apply the applicable Tenant and Environment state, Suppression and hierarchical quota requirements.

Quota evaluation and consumption are per accepted recipient and must comply with [Quota Requirements](quotas.md). Suppression evaluation must comply with [Suppression Requirements](suppressions.md).

## RF-SUB-008 — Durable Acceptance

An accepted SMTP submission must durably record:

- the globally unique Velaris Message identifier;
- the authenticating Credential;
- the owning Tenant and Environment;
- the envelope sender;
- every accepted envelope recipient;
- the submitted message content;
- the acceptance timestamp;
- sufficient non-secret connection and authentication metadata for auditability.

The relational acceptance model must retain the SMTP envelope, message headers, body content and submission metadata. Attachment bytes must be durably stored in Blob/Object Storage before relational acceptance commits, and the Message must retain each attachment's identifier, opaque object reference, filename when supplied, content type, size, hash/checksum and required MIME relationship metadata.

Messages without attachments must not require a Blob/Object Storage operation for acceptance.

The same atomic acceptance operation must create exactly one Delivery for every accepted envelope recipient. Those Deliveries must remain unqueued and unprocessed at the acceptance boundary.

## RF-SUB-009 — Final Reply and Atomic Failure

Velaris must return the final successful SMTP reply only after it has safely accepted responsibility for the Message and satisfied RF-SUB-008.

If durable persistence fails, Velaris must return an appropriate SMTP failure and must not expose a partially accepted Message.

## RF-SUB-010 — Acceptance Boundary

Final SMTP submission success must not depend on queue processing, Delivery Pool selection, MX resolution, an outbound SMTP connection or an outbound Delivery Attempt.

Accepted submission data must remain isolated to the Tenant resolved from the authenticating Credential.

## RF-SUB-011 — Ambiguous Completion and Retransmission

If Velaris commits acceptance but the SMTP connection fails before the client observes the final success reply, the committed Message and Deliveries must remain accepted.

A client retransmission is a new SMTP submission and may create a new Message and Delivery set. Velaris must not deduplicate independent SMTP submissions using only client `Message-ID`, content hash or other heuristic similarity.

Internal retries for the same Velaris work item must remain idempotent by Velaris identifiers.

## Related requirements

- [Message Requirements](messages.md): RF-MSG-002, RF-MSG-005 and RF-MSG-006.
- [Credential Requirements](credentials.md): RF-CRE-003, RF-CRE-004 and RF-CRE-010 through RF-CRE-014.
- [Tenant Requirements](tenants.md): RF-TEN-004 and RF-TEN-015.
- [Environment Requirements](environments.md): RF-ENV-004.
- [Quota Requirements](quotas.md): RF-QUO-001 through RF-QUO-007.
- [Suppression Requirements](suppressions.md): RF-SUP-001 through RF-SUP-003.
- [Access Control Requirements](access-control.md): RF-ACL-015.

## Related architecture decisions

- [ADR-0008](../adr/0008-smtp-tls-termination.md) — SMTP TLS termination.
- [ADR-0009](../adr/0009-smtp-scram-credential-verifiers.md) — SCRAM verifier storage and lifecycle.
- [ADR-0010](../adr/0010-message-and-attachment-persistence.md) — relational Message data, durable attachment storage and failure handling.
- [ADR-0011](../adr/0011-concurrent-quota-acceptance.md) — atomic quota consumption and acceptance.
- [ADR-0012](../adr/0012-submission-configuration-consistency.md) — configuration consistency.
- [ADR-0013](../adr/0013-smtp-at-least-once-submission.md) — retransmission semantics.

## References

- RFC 8314 — Cleartext Considered Obsolete: Use of Transport Layer Security (TLS) for Email Submission and Access: https://www.rfc-editor.org/rfc/rfc8314
- RFC 7677 — SCRAM-SHA-256 and SCRAM-SHA-256-PLUS Simple Authentication and Security Layer (SASL) Mechanisms: https://www.rfc-editor.org/rfc/rfc7677
- RFC 9325 — Recommendations for Secure Use of Transport Layer Security (TLS) and Datagram Transport Layer Security (DTLS): https://www.rfc-editor.org/rfc/rfc9325
- RFC 5321 — Simple Mail Transfer Protocol: https://www.rfc-editor.org/rfc/rfc5321
