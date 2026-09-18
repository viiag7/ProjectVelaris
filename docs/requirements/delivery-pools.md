# Delivery Pool Requirements

A Delivery Pool represents a logical collection of outbound resources and delivery policies. It intentionally abstracts beyond a simple IP pool.

## RF-DPL-001 — Create Delivery Pool

A Platform Administrator must be able to create Delivery Pools.

## RF-DPL-002 — Outbound Resources

A Delivery Pool must be able to contain one or more outbound resources.

Initially these resources may represent source IP addresses. The model should remain extensible to other outbound transports.

## RF-DPL-003 — Delivery Pool Properties

A Delivery Pool may contain configuration such as:

- source IPs;
- HELO/EHLO identity;
- limits;
- routing policies.

## RF-DPL-004 — Tenant Authorization

A Platform Administrator must be able to define which Delivery Pools a Tenant is authorized to use.

## RF-DPL-005 — Default Pool

An Environment must have a default Delivery Pool.

## RF-DPL-006 — Routing Rules

The platform must support selecting a Delivery Pool using routing rules that may consider:

- Tenant;
- Environment;
- sender Domain;
- sender mailbox;
- recipient;
- recipient Domain.
