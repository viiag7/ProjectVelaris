# Tenant Requirements

## RF-TEN-001 — Create Tenant

A Platform Administrator must be able to create a Tenant.

The creation form must be pre-filled with the platform default limits and allow the administrator to override them before creation.

## RF-TEN-002 — Edit Tenant

A Platform Administrator must be able to edit Tenant configuration and limits.

## RF-TEN-003 — Tenant States

A Tenant must support at least:

- `ACTIVE`
- `PAUSED`
- `DISABLED`

## RF-TEN-004 — Pause Tenant

When a Tenant is `PAUSED`:

- no Environment under it may start new deliveries;
- new message submissions must be rejected;
- already queued messages must remain stored;
- queued messages must not be marked failed solely because of the pause.

## RF-TEN-005 — Resume Tenant

When a paused Tenant returns to `ACTIVE`, its active Environments and retained deliveries must become eligible for processing again.

## RF-TEN-010 — Default Limits

The platform must provide default Tenant limits for at least:

- maximum Environments;
- maximum Domains;
- maximum active Credentials;
- sending quota.

## RF-TEN-011 — Per-Tenant Override

A Platform Administrator must be able to override each default limit for an individual Tenant.

## RF-TEN-012 — Environment Limit

The platform must reject creation of an Environment when the Tenant has reached its configured Environment limit.

## RF-TEN-013 — Domain Limit

The platform must reject new Domains when the sum of Domains across all Tenant Environments reaches the Tenant Domain limit.

## RF-TEN-014 — Credential Limit

The platform must reject new active Credentials when the sum of active Credentials across all Tenant Environments reaches the Tenant Credential limit.

## RF-TEN-015 — SMTP Submission Limits

Tenant creation must require configuration of these SMTP submission limits, pre-filled from platform defaults:

- maximum message size;
- maximum recipients per submission;
- maximum SMTP session duration.

Every Environment and SMTP Credential belonging to the Tenant must be constrained by these limits.
