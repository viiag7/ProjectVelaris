# Terminology

## Platform

The complete Velaris installation.

## Tenant

The primary customer and isolation boundary. A Tenant owns members, limits, quotas and one or more Environments.

## Environment

A logical email sending context inside a Tenant. It is not necessarily a physical server.

Examples: `production`, `staging`, `transactional`, `billing`.

## Domain

A sending domain configured inside an Environment. A Domain may contain DKIM and DNS verification state.

## Credential

An authentication credential used by the HTTP API or SMTP submission service.

## Sender Grant

A rule defining which sender identities a Credential may use.

Initial grant types:

- `DOMAIN`: authorizes mailboxes under one exact domain.
- `MAILBOX`: authorizes one exact email address.

Domain grants do not implicitly authorize subdomains.

## Message

The logical message submitted to Velaris.

## Delivery

The delivery unit for one recipient. A Message with ten recipients creates ten Deliveries.

## Delivery Attempt

One outbound attempt for a Delivery. A Delivery may have multiple attempts due to temporary SMTP failures.

## Delivery Pool

A logical set of outbound delivery resources and policies. It replaces the narrower term “IP Pool”.

## Suppression

A rule preventing new delivery to a recipient because of a manual block or delivery-related condition.

## Roles

- `READER`: read access.
- `EDITOR`: modify the authorized resource.
- `ADMIN`: Tenant-level administration, including access management.
