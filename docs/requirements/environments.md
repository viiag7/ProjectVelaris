# Environment Requirements

## RF-ENV-001 — Create Environment

Authorized users must be able to create Environments inside a Tenant while respecting Tenant limits.

## RF-ENV-002 — Edit Environment

Authorized users must be able to edit Environment configuration.

## RF-ENV-003 — Environment States

An Environment must support at least:

- `ACTIVE`
- `PAUSED`
- `DISABLED`

## RF-ENV-004 — Pause Environment

When an Environment is paused:

- no new message submission may be accepted for it;
- queued messages must remain stored;
- no new Delivery Attempt may start.

Other Environments belonging to the same Tenant must remain unaffected.

## RF-ENV-005 — Resume Environment

When a paused Environment returns to `ACTIVE`, retained Deliveries must become eligible for normal processing again.
