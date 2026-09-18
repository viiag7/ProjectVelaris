# Domain Requirements

## RF-DOM-001 — Register Domain

Authorized users must be able to register Domains inside an Environment, subject to Tenant limits.

## RF-DOM-002 — Domain Ownership

A Domain belongs to one Environment and, transitively, to that Environment's Tenant.

## RF-DOM-003 — DKIM

The platform must generate or accept DKIM configuration for each Domain.

## RF-DOM-004 — DKIM Signing

Outbound messages must be signed using the DKIM configuration applicable to the sender Domain.

## RF-DOM-005 — DNS Verification

The platform must be able to verify the DNS configuration required for a sending Domain.

Initial checks should include:

- DKIM;
- SPF;
- Return-Path.

DMARC may be added later.

## RF-DOM-006 — Domain State

A Domain must expose a lifecycle state such as:

- `PENDING`
- `VERIFIED`
- `INVALID`
- `DISABLED`
