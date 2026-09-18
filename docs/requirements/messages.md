# Message Requirements

## RF-MSG-001 — HTTP API Submission

The platform must accept authenticated message submission through an HTTP API.

## RF-MSG-002 — SMTP Submission

The platform must provide an authenticated SMTP submission service.

## RF-MSG-003 — Message Content

The HTTP API must support at least:

- From;
- To;
- CC;
- BCC;
- Reply-To;
- Subject;
- `text/plain`;
- `text/html`;
- attachments;
- permitted custom headers.

## RF-MSG-004 — MIME Submission

Support for submitting a complete MIME/RFC message may be provided.

## RF-MSG-005 — Message Identifier

Every accepted Message must receive a globally unique identifier.

## RF-MSG-006 — Delivery per Recipient

Every recipient must create an independent Delivery entity.

A Message with three recipients therefore creates three Deliveries.

This requirement does not decide whether those Delivery entities are persisted atomically with submission acceptance or materialized later from durably persisted envelope recipients. That timing must be resolved before implementation of the first submission increment.

## RF-MSG-007 — Aggregate Message State

Message state must be derived from the states of its Deliveries.

A Message may be partially delivered; delivery outcome belongs primarily to each Delivery rather than to the Message as a single absolute result.
