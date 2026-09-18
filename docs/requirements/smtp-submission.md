# SMTP Submission Requirements

SMTP submission is the authenticated client-to-Velaris protocol boundary. It is distinct from outbound SMTP delivery to recipient mail systems.

## RF-SUB-001 — Authentication Required

Velaris must require successful SMTP authentication before accepting a Message.

An unauthenticated transaction must be rejected and must not create an accepted Message.

## RF-SUB-002 — Credential-Derived Context

Successful SMTP authentication must resolve exactly one Credential, Environment and Tenant.

The client must not be able to replace that context through SMTP envelope values, message headers or message content.

## RF-SUB-003 — Resource Eligibility

Velaris must reject submission when the Credential is revoked or its Tenant or Environment is not active.

The SMTP reply must distinguish temporary and permanent conditions according to the configured submission policy.

## RF-SUB-004 — Envelope Sender Authorization

Velaris must validate the `MAIL FROM` identity against the authenticating Credential's Sender Grants before accepting message content.

An unauthorized sender must be rejected without creating an accepted Message.

## RF-SUB-005 — Envelope Recipients

Velaris must evaluate each `RCPT TO` command according to submission policy and retain every accepted envelope recipient.

At least one envelope recipient must be accepted before Velaris accepts `DATA`.

## RF-SUB-006 — Submitted Content

After at least one recipient is accepted, Velaris must process a valid `DATA` transaction and preserve the submitted message content separately from SMTP envelope metadata.

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

## RF-SUB-009 — Final Reply and Atomic Failure

Velaris must return the final successful SMTP reply only after it has safely accepted responsibility for the Message and satisfied RF-SUB-008.

If durable persistence fails, Velaris must return an appropriate SMTP failure and must not expose a partially accepted Message.

## RF-SUB-010 — Acceptance Boundary

Final SMTP submission success must not depend on queue processing, Delivery Pool selection, MX resolution, an outbound SMTP connection or an outbound Delivery Attempt.

Accepted submission data must remain isolated to the Tenant resolved from the authenticating Credential.

## Related requirements

- [Message Requirements](messages.md): RF-MSG-002, RF-MSG-005 and RF-MSG-006.
- [Credential Requirements](credentials.md): RF-CRE-003, RF-CRE-004 and RF-CRE-010 through RF-CRE-013.
- [Tenant Requirements](tenants.md): RF-TEN-004.
- [Environment Requirements](environments.md): RF-ENV-004.
- [Quota Requirements](quotas.md): RF-QUO-001 through RF-QUO-007.
- [Suppression Requirements](suppressions.md): RF-SUP-001 through RF-SUP-003.
- [Access Control Requirements](access-control.md): RF-ACL-015.
