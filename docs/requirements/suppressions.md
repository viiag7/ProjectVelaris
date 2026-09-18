# Suppression Requirements

## RF-SUP-001 — Suppression List

The platform must maintain recipient Suppressions that prevent new Delivery processing.

## RF-SUP-002 — Initial Scope

A Suppression is initially scoped to an Environment.

Tenant-wide Suppressions may be evaluated later.

## RF-SUP-003 — Suppression Check

The platform must verify applicable Suppressions before accepting or processing a recipient Delivery.

## RF-SUP-004 — Manual Add

Authorized users must be able to manually add a recipient to the Suppression List.

## RF-SUP-005 — Manual Remove

Authorized users must be able to manually remove a recipient from the Suppression List.

## RF-SUP-006 — Automatic Suppression

Selected permanent delivery failures may cause automatic Suppression according to platform policy.

## RF-SUP-007 — Suppression Data

A Suppression should support at least:

- email;
- reason;
- source;
- SMTP code;
- Enhanced Status Code;
- created timestamp;
- optional expiration timestamp.
