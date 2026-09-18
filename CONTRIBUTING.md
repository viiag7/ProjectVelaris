# Contributing to Project Velaris

Velaris uses an agile, docs-as-code development workflow.

## Before starting work

1. Work must be represented by a GitHub Issue.
2. The Issue should reference applicable RF/RNF/ADR identifiers.
3. The Issue must satisfy the Definition of Ready before entering a sprint.
4. Create a short-lived branch from the current integration branch.
5. Open a Pull Request and link the Issue.

## Work item hierarchy

```text
Product Goal
└── Milestone
    └── Epic
        └── Story
            ├── Task
            └── Task

Spike
Bug
```

- **Epic**: a large outcome spanning multiple Stories.
- **Story**: a user/business outcome deliverable within one sprint.
- **Task**: technical work supporting a Story.
- **Spike**: time-boxed research or experiment that reduces uncertainty.
- **Bug**: behavior that differs from an accepted requirement.

## Branch naming

Use one of:

```text
feat/<issue>-short-description
fix/<issue>-short-description
spike/<issue>-short-description
docs/<issue>-short-description
chore/<issue>-short-description
```

Examples:

```text
feat/42-sender-grant-validation
spike/51-rabbitmq-throughput
fix/73-quota-race-condition
```

## Pull Requests

A Pull Request should:

- be focused on one coherent change;
- link the Issue it implements;
- reference relevant RF/RNF/ADR identifiers;
- include test evidence;
- update documentation when behavior or architecture changes;
- pass the Definition of Done.

Prefer small PRs that can be reviewed independently.

## Commit messages

Prefer Conventional Commit style:

```text
feat: add sender grant validation
fix: prevent quota over-allocation
docs: document delivery retry policy
test: add tenant isolation tests
chore: update build pipeline
```

## Process documentation

See [docs/process/README.md](docs/process/README.md).
