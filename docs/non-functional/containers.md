# Container and Runtime Requirements

## RNF-CTR-001 — Containerized Deployment

**Status:** ACCEPTED

All horizontally scalable Velaris services and workers must be deployable as OCI-compatible containers.

## RNF-CTR-002 — Ephemeral Instances

**Status:** ACCEPTED

Containers must be treated as replaceable and ephemeral.

Loss of a single container filesystem must not cause loss of accepted Messages, delivery state or required configuration.

## RNF-CTR-003 — Non-Root Execution

**Status:** ACCEPTED

Application containers must run as non-root whenever technically possible.

Exceptions require explicit security review.

## RNF-CTR-004 — Privilege Reduction

**Status:** ACCEPTED

Production containers must not run privileged and must minimize Linux capabilities and privilege escalation.

## RNF-CTR-005 — Read-Only Root Filesystem

**Status:** ACCEPTED

Application containers should use a read-only root filesystem where supported.

Writable paths must be explicit and minimized.

## RNF-CTR-006 — Resource Requests and Limits

**Status:** ACCEPTED

Production workloads must define resource requests and bounded resource usage appropriate to their workload.

Resource exhaustion in one workload must not be allowed to destabilize unrelated critical workloads without control.

## RNF-CTR-007 — Health Probes

**Status:** ACCEPTED

Services must expose health signals appropriate for orchestration, including readiness and liveness semantics where applicable.

Readiness must not report healthy when the service cannot safely accept the workload represented by that endpoint.

## RNF-CTR-008 — Graceful Shutdown

**Status:** ACCEPTED

Workers must support graceful termination.

On shutdown they must stop accepting new work, complete or safely release in-flight work, and avoid losing broker acknowledgements or durable state.

## RNF-CTR-009 — Immutable Images

**Status:** ACCEPTED

Production images must be immutable artifacts.

Deployment must identify exact image versions and support digest-based pinning.

## RNF-CTR-010 — Image Supply Chain

**Status:** ACCEPTED

Container images must be produced by controlled build pipelines and support vulnerability scanning and provenance/signature verification.

## RNF-CTR-011 — Network Segmentation

**Status:** ACCEPTED

Containerized workloads must support network policies that restrict unnecessary east-west and north-south connectivity.
