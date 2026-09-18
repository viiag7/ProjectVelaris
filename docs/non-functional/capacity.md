# Capacity Requirements

## RNF-CAP-001 — Outbound Capacity

**Status:** ACCEPTED

The Data Plane must sustain at least **1,000,000 outbound Deliveries per hour** under normal production operating conditions.

Outbound capacity is measured by Delivery/recipient, not by logical Message.

## RNF-CAP-002 — Inbound Capacity

**Status:** ACCEPTED

The Data Plane must sustain at least **1,000,000 inbound email messages per hour** under normal production operating conditions.

## RNF-CAP-003 — Simultaneous Bidirectional Load

**Status:** ACCEPTED

Capacity validation must include simultaneous inbound and outbound processing.

The platform must not be certified using isolated outbound-only or inbound-only load tests.

## RNF-CAP-004 — Backpressure

**Status:** ACCEPTED

When incoming workload temporarily exceeds instantaneous worker processing capacity, the platform must apply backpressure through durable queues rather than losing accepted Messages.

## RNF-CAP-005 — Peak Absorption

**Status:** ACCEPTED

The platform must tolerate temporary workload peaks above nominal processing capacity by buffering accepted work, subject to configured storage and queue safety limits.

No accepted Message may be silently dropped because workers are saturated.

## RNF-CAP-006 — Capacity Test Profile

**Status:** ACCEPTED

Performance certification must include representative message sizes, attachment sizes, recipient counts, inbound MIME complexity, SMTP retries and persistence overhead.

Synthetic tests using only trivial messages are insufficient for production capacity certification.

## RNF-CAP-007 — Capacity Headroom

**Status:** PROPOSED

Production sizing must include defined operational headroom above the nominal one-million-per-hour requirement.

The exact headroom percentage must be established by benchmark before production readiness approval.
