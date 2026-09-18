# ADR-0007: Resource-Level Access Control

**Status:** Accepted

## Context

Users need different access levels across a Tenant and its resources. For example, a user may edit one Environment while only reading another Domain.

## Decision

Velaris uses resource-targeted Access Grants with initial roles:

- `READER`
- `EDITOR`
- `ADMIN`

Reader and Editor can apply to resource levels such as Tenant, Environment and Domain. Admin is initially a Tenant-level administrative role.

Permissions may be inherited from parent resources and refined by more specific grants.

## Consequences

- Access can be delegated narrowly.
- Editor does not automatically grant permission-management capability.
- Credential read access does not reveal stored secrets.
- The authorization engine must compute effective permissions across the resource hierarchy.
