# ADR-0005: Delivery Pool Abstraction

**Status:** Accepted

## Context

The term “IP Pool” is too narrow for a system that may eventually route through multiple outbound resource types and policies.

## Decision

Velaris uses the term **Delivery Pool**.

A Delivery Pool may contain:

- source IP addresses;
- HELO/EHLO identity;
- limits;
- routing policies;
- future outbound resource types.

## Consequences

- The model is not coupled to one-IP-one-route assumptions.
- Tenants can be authorized to use selected pools.
- Environments can define a default pool.
- Routing rules can evolve independently from message submission.
