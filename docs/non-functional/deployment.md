# Deployment and Infrastructure-as-Code Requirements

## RNF-DEP-001 — Infrastructure as Code

**Status:** ACCEPTED

Production infrastructure must be provisionable through version-controlled Infrastructure as Code (IaC).

## RNF-DEP-002 — Infrastructure Coverage

**Status:** ACCEPTED

IaC must cover infrastructure resources required by the deployment where provider APIs permit, including as applicable:

- compute/orchestration resources;
- networks and security controls;
- load balancers;
- databases;
- message-broker infrastructure;
- object storage;
- DNS;
- public/private IP resources;
- monitoring dependencies;
- key/secret infrastructure.

## RNF-DEP-003 — Pull Request Review

**Status:** ACCEPTED

Production infrastructure changes must follow a reviewable change workflow before application.

## RNF-DEP-004 — Continuous Delivery

**Status:** ACCEPTED

Application releases must be deployable through an automated Continuous Delivery pipeline.

## RNF-DEP-005 — Immutable Release Artifacts

**Status:** ACCEPTED

The same immutable build artifact must progress through deployment stages rather than rebuilding different binaries/images for each environment.

## RNF-DEP-006 — Deployment Rollback

**Status:** ACCEPTED

The delivery process must support rollback or forward-recovery for failed releases.

## RNF-DEP-007 — Safe Worker Deployment

**Status:** ACCEPTED

Deploying or replacing workers must not lose accepted work.

Worker termination and broker acknowledgement semantics must be coordinated to permit safe rolling deployment.

## RNF-DEP-008 — Database Migration Coordination

**Status:** ACCEPTED

Deployment pipelines must coordinate database migrations with application compatibility.

Migrations must support rollout strategies that avoid unnecessary full-system downtime.

## RNF-DEP-009 — Environment Reproducibility

**Status:** ACCEPTED

Infrastructure for equivalent deployment environments must be reproducible from version-controlled definitions and explicit configuration.

## RNF-DEP-010 — Drift Control

**Status:** ACCEPTED

Manual production infrastructure changes must be minimized and infrastructure drift must be detectable.

## RNF-DEP-011 — Secret Separation

**Status:** ACCEPTED

IaC source code and deployment manifests must not contain plaintext production secrets.

## RNF-DEP-012 — Security Gates

**Status:** ACCEPTED

CI/CD must support automated quality and security gates before production deployment, including tests and relevant artifact/infrastructure scanning.
