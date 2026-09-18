# Definition of Ready and Done

## Definition of Ready

A Story is Ready for Sprint Planning when:

- [ ] Desired outcome is clear.
- [ ] Acceptance criteria are testable.
- [ ] Applicable RF/RNF/ADR identifiers are linked.
- [ ] Dependencies are known.
- [ ] Security impact has been considered.
- [ ] Reliability/failure behavior has been considered when applicable.
- [ ] Observability requirements are identified when applicable.
- [ ] Data migration implications are identified when applicable.
- [ ] Major technical uncertainty has been resolved or separated into a Spike.
- [ ] Story is sized at 8 or less.
- [ ] Story can reasonably be completed within one sprint.

## Definition of Done

A Story is Done only when applicable items are complete:

### Implementation

- [ ] Acceptance criteria are satisfied.
- [ ] Code has been reviewed through Pull Request.
- [ ] Automated tests have been added or updated.
- [ ] CI checks pass.
- [ ] No known critical regression remains.

### Security

- [ ] Authentication and authorization are enforced server-side.
- [ ] Tenant isolation is preserved.
- [ ] Secrets and sensitive data are not exposed.
- [ ] Relevant security tests pass.

### Reliability

- [ ] Retry/idempotency behavior has been considered for asynchronous processing.
- [ ] Failure and restart behavior are safe.
- [ ] Concurrency behavior is tested where relevant.
- [ ] Database changes are migration-safe.

### Observability

- [ ] Important errors are observable.
- [ ] Required metrics/logging/tracing are present.
- [ ] Correlation identifiers are preserved where applicable.

### Documentation

- [ ] RF/RNF documentation is updated if behavior changed.
- [ ] ADR is added/superseded when an architectural decision changed.
- [ ] Operational documentation/runbook is updated when applicable.

### Delivery

- [ ] Change can be deployed by the supported delivery mechanism.
- [ ] Rollback or forward-recovery implications are understood.
