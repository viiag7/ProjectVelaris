# ADR-0004: Credential Sender Grants

**Status:** Accepted

## Context

A Credential may need to send as an entire Domain, one Mailbox, several Mailboxes or a combination of these.

Credential authentication alone is insufficient to decide whether a sender identity is authorized.

## Decision

Credentials contain one or more Sender Grants.

Initial grant types:

- `DOMAIN`
- `MAILBOX`

A Domain grant applies only to the exact Domain and does not automatically include subdomains.

## Consequences

- Authentication and sender authorization remain separate.
- One Credential can represent flexible sender policies.
- Unauthorized senders are rejected before queue admission.
- Additional grant types may be added without creating new Credential types.
