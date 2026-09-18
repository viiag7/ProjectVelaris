# Authentication Requirements

## RNF-AUT-001 — OpenID Connect for Human Authentication

**Status:** ACCEPTED

Human authentication for the Velaris Control Plane must use **OpenID Connect (OIDC)**.

Velaris must not require a proprietary application-specific username/password authentication protocol for Control Plane users.

## RNF-AUT-002 — OIDC Token Validation

**Status:** ACCEPTED

Services accepting OIDC access or identity tokens must validate all security-relevant token properties, including as applicable:

- signature;
- issuer;
- audience;
- expiration;
- not-before time;
- token type and authorized algorithms.

Tokens that fail validation must be rejected.

## RNF-AUT-003 — Stable External Identity

**Status:** ACCEPTED

A federated user identity must be associated using the OIDC issuer and subject identifier.

Email address must not be used as the immutable primary identity key.

Conceptually:

```text
ExternalIdentity
  issuer
  subject
  -> Velaris User
```

## RNF-AUT-004 — Strong Authentication

**Status:** ACCEPTED

The authentication architecture must support multi-factor and phishing-resistant authentication through the configured OpenID Provider.

Sensitive administrative operations must be able to require stronger authentication or step-up authentication.

## RNF-AUT-005 — Authentication Context

**Status:** ACCEPTED

Where provided by the OpenID Provider, authentication context such as `acr` and `amr` must be available to authorization policy for sensitive operations.

## RNF-AUT-006 — Separation of Human and Machine Authentication

**Status:** ACCEPTED

Human OIDC identities must be distinct from machine Credentials used for HTTP Submission and SMTP Submission.

A user access token must not implicitly act as a sending Credential unless an explicit API feature is designed for that purpose.

## RNF-AUT-007 — API Credential Secret Protection

**Status:** ACCEPTED

API Credential secrets must have sufficient cryptographic entropy, support revocation and rotation, and must not be stored in recoverable plaintext when only verification is required.

The complete secret should be displayed only at creation or rotation time.

## RNF-AUT-008 — SMTP Credential Protection

**Status:** ACCEPTED

SMTP authentication secrets must be protected with the same secret-management principles as API Credentials.

Credential exchange must only occur over a transport configuration approved for production security.

When SCRAM verification is used, Velaris must retain only the derived verifier material and parameters required for authentication rather than a recoverable client password. Creation, rotation and revocation must not require recovery of an earlier secret.

## RNF-AUT-009 — Revocation

**Status:** ACCEPTED

Revoked machine Credentials must cease being accepted within a defined and bounded propagation interval.

The propagation interval must be measurable and documented before production readiness.

## RNF-AUT-010 — Workload Identity

**Status:** ACCEPTED

Internal services and workers must use workload/service identities instead of shared human credentials.

Critical service-to-service authentication must not depend on passwords manually embedded into container configuration.

## RNF-AUT-011 — Identity Auditability

**Status:** ACCEPTED

Authentication events relevant to security investigations must be auditable without recording reusable secrets or raw bearer tokens.
