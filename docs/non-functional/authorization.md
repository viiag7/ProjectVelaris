# Authorization Requirements

Velaris authorization is resource-oriented and hierarchical.

The authorization contract between Policy Enforcement Points (PEPs) and the Policy Decision Point (PDP) must use the **OpenID AuthZEN Authorization API**.

## RNF-AUZ-001 — AuthZEN Authorization Contract

**Status:** ACCEPTED

Protected Control Plane operations must use AuthZEN-compatible authorization decisions between the application PEP and the authorization PDP.

## RNF-AUZ-002 — Subject / Action / Resource / Context

**Status:** ACCEPTED

Authorization decisions must be expressible using the AuthZEN model:

- Subject;
- Action;
- Resource;
- optional Context.

Context must not be treated as an authoritative substitute for server-side resource ownership data.

## RNF-AUZ-003 — Resource Hierarchy

**Status:** ACCEPTED

Authorization policy must support the Velaris resource hierarchy.

At minimum:

```text
Tenant
└── Environment
    ├── Domain
    ├── Credential
    ├── Message
    └── Suppression
```

Other resource types must be incorporable without redesigning the authentication model.

## RNF-AUZ-004 — Roles

**Status:** ACCEPTED

The initial authorization model must support:

- `READER`;
- `EDITOR`;
- `ADMIN`.

`ADMIN` is initially a Tenant-level administrative role.

## RNF-AUZ-005 — Roles Are Not Actions

**Status:** ACCEPTED

Applications must request authorization for concrete actions rather than asking only whether a subject has a role.

Examples of actions include:

- `read`;
- `create`;
- `update`;
- `delete`;
- `pause`;
- `resume`;
- `manage_access`;
- `manage_members`;
- `rotate_credential`;
- `rotate_dkim`.

Roles are policy abstractions that map to actions.

## RNF-AUZ-006 — Reader Semantics

**Status:** ACCEPTED

A `READER` grant permits read-only operations for the target resource according to policy.

Reader must not implicitly grant resource mutation or access-administration actions.

## RNF-AUZ-007 — Editor Semantics

**Status:** ACCEPTED

An `EDITOR` grant permits read and ordinary resource-management actions for the target resource according to policy.

Editor must not automatically grant `manage_access` or `manage_members`.

## RNF-AUZ-008 — Tenant Admin Semantics

**Status:** ACCEPTED

A Tenant `ADMIN` must have administrative authority across the Tenant and its child resources, including access-management operations.

A more specific Reader or Editor grant on a child resource must not reduce the authority of a Tenant ADMIN.

## RNF-AUZ-009 — Inheritance

**Status:** ACCEPTED

Reader and Editor grants assigned to a parent resource must be inheritable by child resources according to the resource hierarchy.

## RNF-AUZ-010 — Specific Resource Override

**Status:** ACCEPTED

For non-ADMIN roles, a direct Reader or Editor grant on a specific child resource must take precedence over an inherited Reader or Editor grant for that resource.

This allows a resource-specific grant to either increase or reduce ordinary inherited access.

## RNF-AUZ-011 — No Explicit Deny in Initial Model

**Status:** ACCEPTED

The initial authorization model must not require explicit deny grants.

When no applicable permission grants an action, the result is deny-by-default.

## RNF-AUZ-012 — Deny by Default

**Status:** ACCEPTED

If the PDP cannot establish an applicable permission for a protected operation, authorization must be denied.

## RNF-AUZ-013 — Fail Closed

**Status:** ACCEPTED

When a required real-time authorization decision cannot be obtained reliably, the Control Plane must fail closed for the protected operation.

Authorization infrastructure failure must never result in implicit allow.

## RNF-AUZ-014 — PEP Enforcement

**Status:** ACCEPTED

Every protected Control Plane operation must enforce authorization at the server-side Policy Enforcement Point.

UI visibility or disabled buttons must never be considered an authorization control.

## RNF-AUZ-015 — Resource Ownership Is Authoritative Server Data

**Status:** ACCEPTED

Tenant, Environment and parent-resource relationships used in authorization must be resolved from authoritative server-side data.

A client-provided `tenant_id` or parent identifier must never be trusted as proof of ownership.

## RNF-AUZ-016 — Collection Authorization

**Status:** ACCEPTED

Collection and search APIs must not disclose resources the subject is unauthorized to read.

Authorization-aware resource filtering must scale without requiring an unbounded number of per-item remote authorization calls.

## RNF-AUZ-017 — Authorization Audit

**Status:** ACCEPTED

Security-sensitive authorization decisions and permission changes must be auditable with sufficient identifiers to reconstruct:

- subject;
- action;
- resource;
- decision;
- policy/configuration version where available;
- timestamp.

## RNF-AUZ-018 — Data Plane Hot Path Independence

**Status:** ACCEPTED

AuthZEN remote authorization must not be required for each outbound Delivery or SMTP protocol interaction.

Machine Credential authorization, Sender Grants, Tenant/Environment state and related Data Plane policies must be materialized or cached for high-throughput processing.

## RNF-AUZ-019 — Configuration Revocation Propagation

**Status:** ACCEPTED

Changes that revoke Data Plane authorization, including Credential revocation and sender authorization removal, must propagate within a defined and measurable maximum interval.

## RNF-AUZ-020 — Secret Visibility

**Status:** ACCEPTED

Authorization to read a Credential resource must not imply authorization to retrieve an existing reusable secret.

Secrets require separate security treatment from resource metadata.
