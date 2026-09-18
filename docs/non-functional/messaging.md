# Messaging Requirements

## RNF-MQ-001 — Durable Critical Work

**Status:** ACCEPTED

Work representing an accepted Message or other critical processing step must use durable messaging semantics.

## RNF-MQ-002 — Publisher Confirmation

**Status:** ACCEPTED

Critical publishers must obtain confirmation that a message has been durably accepted by the messaging infrastructure before considering publication successful.

## RNF-MQ-003 — Manual Consumer Acknowledgement

**Status:** ACCEPTED

Critical consumers must acknowledge work only after the corresponding durable processing step has completed successfully.

## RNF-MQ-004 — Redelivery

**Status:** ACCEPTED

If a worker fails before acknowledgement, the messaging layer must permit the work to be redelivered.

## RNF-MQ-005 — Idempotent Consumers

**Status:** ACCEPTED

Workers must be designed for at-least-once processing and must prevent duplicate side effects when the same logical work item is delivered more than once.

## RNF-MQ-006 — High Availability

**Status:** ACCEPTED

Critical queues must use a messaging topology that tolerates failure of an individual broker node without silent message loss.

## RNF-MQ-007 — Poison Message Protection

**Status:** ACCEPTED

The platform must detect repeatedly failing work and prevent it from creating an infinite immediate-redelivery loop.

Failed work must remain diagnosable and recoverable according to policy.

## RNF-MQ-008 — Backpressure and Prefetch

**Status:** ACCEPTED

Consumer concurrency and prefetch/in-flight work must be bounded and configurable per workload type.

A worker must not reserve substantially more work than it can safely process.

## RNF-MQ-009 — Queue Observability

**Status:** ACCEPTED

The platform must expose at least:

- queue depth;
- unacknowledged work;
- publish rate;
- consume rate;
- redelivery rate;
- age of oldest work item where available;
- unavailable/failed consumers.

## RNF-MQ-010 — Broker Security

**Status:** ACCEPTED

Message-broker identities must be separated by service or responsibility, use least privilege and use authenticated encrypted connections in production.

## RNF-MQ-011 — Large Backlog Safety

**Status:** ACCEPTED

Messaging topology selection and configuration must be validated against worst-case backlog size, message size and recovery behavior.

Nominal throughput alone is insufficient for broker sizing.

## RNF-MQ-012 — Controlled Retry Scheduling

**Status:** ACCEPTED

Delayed retries must not produce tight retry loops that amplify downstream failures.

Retry scheduling must support bounded, observable delay policies.
