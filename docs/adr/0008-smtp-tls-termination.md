# ADR-0008: SMTP TLS Terminates in the Submission Service

**Status:** Accepted

## Context

Story #4 requires implicit TLS and `SCRAM-SHA-256-PLUS`. The `-PLUS` mechanism binds authentication to the TLS session, so the component validating SCRAM must have access to the channel-binding data produced by that same TLS session.

Velaris must also scale the SMTP Submission service horizontally without making one proxy instance the owner of application session state.

## Decision

The SMTP TLS connection terminates directly in the Velaris SMTP Submission service.

A Layer 4 TCP load balancer may distribute connections across service instances, but it must pass the TCP stream through without terminating or re-encrypting the SMTP TLS session. Each accepted TCP connection remains on one SMTP service instance for its lifetime.

The SMTP service owns:

- the server certificate and private-key use through the approved secret-management boundary;
- TLS protocol and cipher configuration;
- TLS channel-binding extraction for `SCRAM-SHA-256-PLUS`;
- SMTP session state;
- graceful connection draining during deployment.

TLS 1.3 is preferred and TLS 1.2 is the minimum supported version, as required by RF-SUB-001.

## Consequences

- SCRAM channel binding is validated against the actual client-to-Velaris TLS session.
- Layer 7 TLS termination for SMTP Submission is not permitted.
- Certificate and key rotation must reach every SMTP service instance without embedding private keys in application images or source code.
- Load-balancer health checks must not require TLS termination by the load balancer.
- Horizontal scaling remains possible through Layer 4 connection distribution.
- Rolling deployment must drain existing SMTP connections before terminating an instance.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- RF-SUB-001
- RNF-AUT-008
- RNF-SEC-005, RNF-SEC-007 and RNF-SEC-008
- RNF-SCA-006
- RNF-AVL-005
