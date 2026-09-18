# ADR-0009: SMTP Credentials Store SCRAM Verifiers

**Status:** Accepted; mechanism selection amended by ADR-0015

## Context

Story #4 uses `SCRAM-SHA-256-PLUS` for SMTP authentication. Velaris must validate a Credential without retaining a recoverable SMTP password. Credential creation, rotation and revocation must remain possible without recovering the original secret.

## Decision

An SMTP Credential stores the material required to verify SCRAM authentication, not the client secret itself.

The protected representation contains at least:

- Credential identifier, Tenant and Environment ownership;
- lifecycle state and verifier version;
- SCRAM mechanism identifier;
- salt;
- iteration count and derivation parameters;
- `StoredKey`;
- `ServerKey`;
- creation, rotation and revocation metadata.

The lifecycle is:

```text
Create -> Active -> Rotate -> Active -> Revoke
```

On creation or rotation, Velaris generates or accepts the new secret through an approved write-only boundary, derives the SCRAM verifier material and discards the recoverable secret. The complete secret may be displayed or returned only during that creation or rotation operation.

Rotation creates a new verifier version and atomically makes the previous version ineligible for new authentication. Revocation makes every verifier version for that Credential ineligible. A submission revalidates the active Credential and authenticated verifier version before final acceptance so a session authenticated before rotation or revocation cannot later accept a Message with stale authority.

Iteration parameters are stored per verifier version so security parameters can evolve without retaining the original secret.

## Consequences

- A stored verifier cannot be used to recover or display the SMTP password through an ordinary read path.
- Losing the client secret requires rotation rather than recovery.
- Rotation and revocation require no access to the previous secret.
- Authentication and acceptance paths must use constant-time comparisons where applicable and must not log secrets, SCRAM proofs or reusable authentication material.
- Changing to an authentication mechanism that requires different verifier material may require Credential rotation.
- The Control Plane needs an auditable create, rotate and revoke workflow.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- RF-CRE-001, RF-CRE-003, RF-CRE-012 and RF-CRE-014
- RF-SUB-001 through RF-SUB-003
- RNF-AUT-007 through RNF-AUT-009 and RNF-AUT-011
- RNF-SEC-006 through RNF-SEC-009
