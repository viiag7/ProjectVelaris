# Observability Requirements

## RNF-OBS-001 — Structured Logging

**Status:** ACCEPTED

Services must emit structured logs suitable for centralized querying and machine processing.

## RNF-OBS-002 — Correlation Identifiers

**Status:** ACCEPTED

Operational telemetry must preserve correlation identifiers where applicable, including:

- Tenant ID;
- Environment ID;
- Message ID;
- Delivery ID;
- Delivery Attempt ID;
- request/correlation ID.

Sensitive identifiers must be handled according to security and privacy policy.

## RNF-OBS-003 — Metrics

**Status:** ACCEPTED

Critical services must expose operational metrics sufficient to measure throughput, failures, latency, saturation and backlog.

## RNF-OBS-004 — Distributed Tracing

**Status:** ACCEPTED

Synchronous and asynchronous critical flows must support distributed tracing or equivalent correlation across service boundaries.

## RNF-OBS-005 — Submission Metrics

**Status:** ACCEPTED

The platform must expose metrics for HTTP and SMTP submission including accepted, rejected, throttled and failed submissions.

## RNF-OBS-006 — Delivery Metrics

**Status:** ACCEPTED

The platform must expose at least:

- Deliveries created;
- Deliveries delivered;
- Deliveries deferred;
- Deliveries failed;
- Deliveries expired;
- retry counts;
- SMTP response classes;
- Delivery processing latency.

## RNF-OBS-007 — Inbound Metrics

**Status:** ACCEPTED

Inbound processing must expose throughput, rejection, parsing, scanning and processing-failure metrics.

## RNF-OBS-008 — Queue Metrics

**Status:** ACCEPTED

Queue depth, backlog age and processing rate must be observable for critical queues.

## RNF-OBS-009 — Quota Metrics

**Status:** ACCEPTED

Tenant and Environment quota consumption must be observable and suitable for threshold alerting.

## RNF-OBS-010 — Health Endpoints

**Status:** ACCEPTED

Critical services must expose health endpoints or equivalent platform health signals.

## RNF-OBS-011 — Alerting

**Status:** ACCEPTED

Production environments must support alerting for conditions including:

- service unavailability;
- growing queue backlog;
- processing stall;
- database saturation;
- messaging degradation;
- quota subsystem errors;
- abnormal delivery failure rates;
- backup failures.

## RNF-OBS-012 — Audit/Telemetry Separation

**Status:** ACCEPTED

Security audit records must not rely exclusively on ordinary application logs that may have shorter retention or different access controls.
