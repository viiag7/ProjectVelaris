# Scalability Requirements

## RNF-SCA-001 — Horizontal Worker Scaling

**Status:** ACCEPTED

Data Plane processing components must support horizontal scaling through multiple worker instances.

## RNF-SCA-002 — Container-Oriented Workers

**Status:** ACCEPTED

Workers must be deployable as containers and designed so instances can be started, stopped and replaced without manual redistribution of work.

## RNF-SCA-003 — Stateless Processing

**Status:** ACCEPTED

Workers should remain stateless with durable state stored in external persistence or messaging systems.

Local container filesystem state must not be required for recovery of accepted Messages.

## RNF-SCA-004 — Concurrent Consumption

**Status:** ACCEPTED

Multiple workers must be able to consume work concurrently without duplicate side effects, lost state transitions or quota corruption.

## RNF-SCA-005 — Autoscaling Signals

**Status:** ACCEPTED

Autoscaling must be able to use workload signals beyond CPU and memory, including where applicable:

- queue depth;
- age of oldest queued item;
- processing rate;
- retry backlog;
- inbound backlog.

## RNF-SCA-006 — Independent Scaling

**Status:** ACCEPTED

Submission, outbound delivery, inbound reception, parsing and document-processing workloads must be scalable independently.

## RNF-SCA-007 — Scale Without Repartitioning by Operator

**Status:** ACCEPTED

Adding worker capacity must not require an operator to manually move existing Messages between workers.
