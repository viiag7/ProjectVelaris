# Access Control Requirements

Authorization is resource-based and hierarchical.

## RF-ACL-001 — Tenant Membership

Users must be associable with one or more Tenants.

## RF-ACL-002 — Reader

The `READER` role permits viewing the authorized resource and its non-secret data but not modifying it.

## RF-ACL-003 — Editor

The `EDITOR` role permits viewing and modifying the authorized resource according to operations supported by that resource type.

## RF-ACL-004 — Tenant Admin

The `ADMIN` role exists at Tenant level and permits:

- managing Tenant members;
- managing permissions;
- administering Tenant resources;
- viewing Tenant limits and quotas.

## RF-ACL-005 — Tenant Grant

A user may receive a role directly on a Tenant.

## RF-ACL-006 — Environment Grant

A user may receive a role directly on a specific Environment.

## RF-ACL-007 — Domain Grant

A user may receive a role directly on a specific Domain.

## RF-ACL-008 — Credential Access

Credential resources must support Reader and Editor access, but read access must never imply disclosure of an existing secret.

## RF-ACL-009 — Extensible Resource Grants

The authorization model must support grants for additional resource types such as Delivery Pools, Messages and Suppressions.

## RF-ACL-010 — Inheritance

Roles assigned to a parent resource may be inherited by its child resources.

## RF-ACL-011 — Specific Grants

A role explicitly assigned to a more specific resource must be considered when resolving effective access and may grant more specific access than an inherited role.

Example:

```text
Tenant ACME
  João -> READER

Environment staging
  João -> EDITOR
```

The effective role is Reader for the rest of the Tenant and Editor for `staging`.

## RF-ACL-012 — Effective Permission Resolution

Effective authorization must consider:

1. direct grants;
2. inherited grants;
3. resource-specific grants;
4. administrative role.

## RF-ACL-013 — Access Administration

Ordinary Editors must not automatically be allowed to grant permissions to other users. Access administration requires explicit administrative authority.

## RF-ACL-014 — Last Administrator Protection

The system must not allow removal or demotion of the last active Tenant Administrator.

## RF-ACL-015 — Tenant Isolation

A user must not access resources belonging to a Tenant without an authorized membership or grant.

## RF-ACL-016 — Access Audit

Permission changes must record at least:

- actor;
- affected user;
- resource;
- previous role;
- new role;
- timestamp.
