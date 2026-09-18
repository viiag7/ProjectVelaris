# Scope

## First usable increment

The first increment provides authenticated SMTP submission through durable acceptance:

```text
authenticate
  -> derive Tenant and Environment from the Credential
  -> authorize sender
  -> apply submission policy
  -> safely persist the Message, envelope, content and audit metadata
  -> return final SMTP success
  -> stop
```

Queue processing, Delivery Pool selection, MX resolution, outbound SMTP connections and Delivery Attempts are excluded from this increment. They remain part of the broader initial scope below.

## Initial scope

The initial Velaris scope includes:

- Platform administration.
- Tenant management.
- Environment management.
- Tenant and Environment quotas.
- Resource-level authorization.
- Domain and DKIM configuration.
- HTTP API submission.
- SMTP submission.
- Credentials and Sender Grants.
- Queueing.
- Per-recipient Deliveries.
- SMTP Delivery Attempts and retries.
- Delivery Pools.
- Message history.
- Suppressions.
- Basic operational dashboards.

## Explicitly deferred

The following capabilities are relevant but intentionally deferred:

- Webhooks.
- Open tracking.
- Click tracking.
- Inbound email.
- Inbound routing and MIME parsing.

See [Future scope](../future/).
