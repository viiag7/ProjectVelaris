# Inbound Processing Security Requirements

Inbound email and attachments are untrusted input.

These requirements are especially important because Velaris may receive NF-e, CT-e and other business/fiscal documents.

## RNF-INB-001 — Untrusted Input Boundary

**Status:** ACCEPTED

All inbound SMTP content, MIME structures, headers, attachments and document payloads must be treated as untrusted input.

## RNF-INB-002 — Message Size Limits

**Status:** ACCEPTED

Inbound SMTP must enforce configurable maximum message and attachment sizes before unbounded resource consumption occurs.

## RNF-INB-003 — Parser Resource Limits

**Status:** ACCEPTED

MIME, compression and document parsing must enforce bounded CPU, memory, recursion/nesting and output-size limits.

## RNF-INB-004 — Decompression Bomb Protection

**Status:** ACCEPTED

Compressed attachments must be processed with controls against decompression bombs and excessive expansion ratios.

## RNF-INB-005 — Malware Scanning

**Status:** ACCEPTED

The inbound pipeline must support malware scanning or equivalent security inspection before untrusted attachments are exposed to downstream business processing.

## RNF-INB-006 — Parser Isolation

**Status:** ACCEPTED

Document parsing and other risky content-processing workloads must be isolated from the SMTP ingress path so expensive or malicious content does not unnecessarily block email reception.

## RNF-INB-007 — XML External Entity Protection

**Status:** ACCEPTED

XML parsers used for NF-e, CT-e or other untrusted documents must disable unsafe external entity and external DTD processing unless an explicitly reviewed requirement mandates otherwise.

## RNF-INB-008 — No Uncontrolled Parser Egress

**Status:** ACCEPTED

Untrusted content parsing must not be able to initiate arbitrary outbound network connections.

## RNF-INB-009 — Attachment Storage

**Status:** ACCEPTED

Untrusted attachments and raw MIME content must be persisted in controlled durable storage rather than relying on long-lived local container filesystem paths.

## RNF-INB-010 — Content Integrity

**Status:** ACCEPTED

The platform must preserve sufficient integrity metadata to detect corruption or unintended modification of stored inbound content.

## RNF-INB-011 — Failure Quarantine

**Status:** ACCEPTED

Content that cannot be safely parsed or scanned must be quarantinable for controlled investigation and must not enter ordinary downstream document-processing workflows as trusted content.

## RNF-INB-012 — Inbound Backpressure

**Status:** ACCEPTED

Inbound reception and downstream parsing must be decoupled by durable asynchronous processing so parsing saturation does not silently lose accepted inbound mail.
