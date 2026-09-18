# ADR-0015: SMTP Authentication Uses SCRAM-SHA-256 over Implicit TLS

**Status:** Accepted

## Context

Story #4 originally selected `SCRAM-SHA-256-PLUS` with TLS channel binding. ADR-0008 consequently required the SMTP service to extract channel-binding data from its TLS session, while ADR-0009 selected non-recoverable SCRAM verifier storage.

ADR-0014 selects .NET 10 for SMTP Submission. The public .NET 10 `SslStream` API does not expose the RFC 9266 `tls-exporter` keying material required for standards-compliant SCRAM channel binding over TLS 1.3. Using a custom TLS stack or native TLS interop only to recover that value would materially increase cryptographic ownership, upgrade risk and operational complexity in the first increment.

Velaris already requires implicit TLS, rejects plaintext submission and does not cross a plaintext-to-`STARTTLS` transition. The product owner approved revising the initial SMTP authentication mechanism while preserving encrypted transport and non-recoverable Credential storage.

## Decision

The first SMTP Submission implementation uses `SCRAM-SHA-256` over mandatory implicit TLS.

- TLS terminates in the SMTP Submission service behind optional Layer 4 TCP load balancing.
- TLS 1.3 is preferred and TLS 1.2 is the minimum.
- Plaintext submission and `STARTTLS` upgrade mode are not offered.
- `SCRAM-SHA-256` is the only initial SASL authentication mechanism.
- `SCRAM-SHA-256-PLUS`, `PLAIN` and `LOGIN` are not advertised or accepted in the first implementation.
- SMTP passwords remain non-recoverable. Credential storage and lifecycle continue to use the salt, iteration parameters, `StoredKey`, `ServerKey` and version metadata defined by ADR-0009.
- Authentication comparisons and proof validation use constant-time operations where applicable.
- TLS certificate validation, certificate rotation and secret management remain mandatory security boundaries.

This ADR supersedes only:

- the `SCRAM-SHA-256-PLUS` and channel-binding parts of ADR-0008; and
- the selected SCRAM mechanism name in ADR-0009.

All other decisions in ADR-0008 and ADR-0009 remain accepted.

## Consequences

- The implementation can use the supported .NET 10 TLS stack without custom native or cryptographic interop.
- A TLS exporter is no longer required for the first increment.
- The client secret is still not sent as plaintext SASL credentials and is not stored recoverably by Velaris.
- Removing `-PLUS` removes cryptographic binding between the SCRAM exchange and the exact TLS connection. Mandatory implicit TLS, certificate validation, direct service TLS termination and the absence of weaker SASL fallbacks reduce, but do not eliminate, that difference.
- The SMTP engine must support a multi-step server-side `SCRAM-SHA-256` exchange; packages limited to `PLAIN` and `LOGIN` remain unsuitable as-is.
- Future adoption of `SCRAM-SHA-256-PLUS`, OAuth-based SMTP authentication or client certificates requires a separate security and interoperability review.

## Traceability

- Story #4 — Accept and persist authenticated SMTP submission
- RF-CRE-003, RF-CRE-012 and RF-CRE-014
- RF-SUB-001 through RF-SUB-003
- RNF-AUT-008 and RNF-AUT-009
- RNF-SEC-005 through RNF-SEC-008, RNF-SEC-010 and RNF-SEC-011
- ADR-0008, ADR-0009 and ADR-0014

## References

- RFC 7677 — SCRAM-SHA-256 and SCRAM-SHA-256-PLUS: https://www.rfc-editor.org/rfc/rfc7677
- RFC 8314 — Cleartext Considered Obsolete: https://www.rfc-editor.org/rfc/rfc8314
- RFC 9325 — Recommendations for Secure Use of TLS and DTLS: https://www.rfc-editor.org/rfc/rfc9325
