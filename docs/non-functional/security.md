# Security Requirements

## RNF-SEC-001 — Secure-by-Default

**Status:** ACCEPTED

Production defaults must prefer secure behavior. Insecure compatibility modes must require explicit configuration.

## RNF-SEC-002 — Least Privilege

**Status:** ACCEPTED

Users, workloads, database roles, message-broker identities and infrastructure identities must receive only the minimum permissions required for their responsibilities.

## RNF-SEC-003 — Deny by Default

**Status:** ACCEPTED

Access to protected resources must be denied unless an applicable authorization policy explicitly permits the requested action.

## RNF-SEC-004 — Tenant Isolation

**Status:** ACCEPTED

All data-access paths must enforce Tenant isolation.

Cross-Tenant access must be treated as a critical security failure and covered by automated negative tests.

## RNF-SEC-005 — Encryption in Transit

**Status:** ACCEPTED

External and internal traffic carrying credentials, tokens, message content or administrative data must use authenticated encryption in transit where the participating protocol supports it.

## RNF-SEC-006 — Sensitive Data in Logs

**Status:** ACCEPTED

Logs, traces and metrics must not expose:

- passwords;
- API secrets;
- authentication tokens;
- private cryptographic keys;
- full sensitive document contents unless explicitly designed and access-controlled for that purpose.

## RNF-SEC-007 — Secret Management

**Status:** ACCEPTED

Secrets must not be embedded in source code, container images or version-controlled configuration.

Production secrets must be managed through an auditable secret-management or key-management mechanism.

## RNF-SEC-008 — Secret Rotation

**Status:** ACCEPTED

Operational credentials and cryptographic material must support rotation without requiring destructive reconfiguration of the platform.

## RNF-SEC-009 — Administrative Audit

**Status:** ACCEPTED

Security-sensitive administrative actions must generate immutable or tamper-evident audit records with actor, action, resource, result and timestamp.

## RNF-SEC-010 — Security Baselines

**Status:** ACCEPTED

Security configuration for each infrastructure component must be reviewed against current vendor guidance and applicable industry security baselines before production use.

This includes, where applicable, database, message broker, container runtime, orchestration platform, identity provider, object storage and network configuration.

## RNF-SEC-011 — Vulnerability Management

**Status:** ACCEPTED

Production build and deployment pipelines must support dependency, container-image and infrastructure security scanning.

Critical findings must have a documented remediation or risk-acceptance process.

## RNF-SEC-012 — Cryptographic Key Protection

**Status:** ACCEPTED

Private cryptographic keys, including DKIM and platform signing keys, must be protected using an appropriate secret/key-management solution and must never be exposed through ordinary read APIs.
