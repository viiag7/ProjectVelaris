# Credential Requirements

## RF-CRE-001 — Create Credential

Authorized users must be able to create multiple Credentials inside an Environment.

## RF-CRE-002 — Credential Types

The initial Credential types are:

- HTTP API;
- SMTP.

## RF-CRE-003 — Revoke Credential

A Credential must be revocable independently without affecting other Credentials in the Environment.

## RF-CRE-004 — Sender Grants

Every Credential must have one or more Sender Grants defining which sender identities it may use.

## RF-CRE-005 — Domain Grant

A `DOMAIN` Sender Grant authorizes any mailbox under one exact Domain.

Example:

```text
type: DOMAIN
value: example.com
```

Allows:

```text
billing@example.com
support@example.com
no-reply@example.com
```

## RF-CRE-006 — Subdomain Isolation

A grant for `example.com` must not automatically authorize `mail.example.com`.

Subdomains require explicit authorization.

## RF-CRE-007 — Mailbox Grant

A `MAILBOX` Sender Grant authorizes one exact email address.

Example:

```text
type: MAILBOX
value: billing@example.com
```

## RF-CRE-008 — Multiple Mailboxes

A Credential may contain multiple `MAILBOX` Sender Grants.

## RF-CRE-009 — Combined Grants

A Credential may contain multiple grants of different supported types.

## RF-CRE-010 — Sender Validation

Before accepting a Message, the platform must verify that the submitted sender is covered by at least one Sender Grant belonging to the authenticating Credential.

## RF-CRE-011 — Unauthorized Sender

A submission using an unauthorized sender must be rejected before entering the delivery queue.

## RF-CRE-012 — Credential Secret

Credential secrets must be treated as sensitive data.

Read access to a Credential must not imply the ability to retrieve its existing secret in plain text.

When verification does not require the original secret, Velaris must not store it in recoverable form. SMTP Credentials using SCRAM must store the salt, derivation parameters, `StoredKey`, `ServerKey` and required verifier metadata instead of the recoverable password.

## RF-CRE-013 — Envelope Sender

When a client is allowed to provide SMTP `MAIL FROM` directly, the platform must either validate that envelope identity according to the Environment policy or replace it with a platform-controlled Return-Path.

## RF-CRE-014 — Rotate Credential

An active Credential must support secret rotation without recovering its previous secret.

Rotation must activate new verification material and make the previous verifier ineligible for new authentication. Revocation must make every verifier version for the Credential ineligible.
