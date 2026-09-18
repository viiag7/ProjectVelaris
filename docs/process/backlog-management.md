# Backlog Management

## Work item types

### Epic

Represents a significant product or platform outcome and usually spans multiple sprints.

An Epic should be decomposed into Stories before implementation.

### Story

Represents a demonstrable outcome that should normally fit within one sprint.

Suggested form:

> As a <actor>, I want <capability>, so that <outcome>.

Technical Stories are acceptable when there is no meaningful end-user actor, provided the outcome and acceptance criteria are clear.

### Task

Represents technical work supporting a Story.

Tasks should not replace the Story's outcome or acceptance criteria.

### Spike

A time-boxed investigation used to reduce technical uncertainty.

### Bug

A defect where actual behavior differs from accepted requirements or intended behavior.

## Priority

Use:

| Priority | Meaning |
|---|---|
| P0 | Production-critical incident, data loss/security risk, or work blocking all meaningful progress |
| P1 | Critical path for current milestone/sprint goal |
| P2 | Important but not blocking current critical path |
| P3 | Useful improvement / lower urgency |

P0 must remain exceptional.

## Story size

Use relative sizing:

```text
1, 2, 3, 5, 8
```

Interpretation is relative complexity/risk, not hours.

- **1**: trivial, well understood.
- **2**: small.
- **3**: normal Story.
- **5**: significant complexity or integration.
- **8**: large/high-risk but still expected to fit a sprint.

Anything larger than 8 should normally be split or preceded by a Spike.

Velocity is an internal planning signal and must not be used as an individual performance metric.

## Backlog ordering

Order work using:

1. safety/security risk;
2. milestone critical path;
3. architectural uncertainty;
4. customer/product value;
5. dependency unlocking;
6. cost of delay.

For Velaris, high-risk architectural assumptions should be validated early.

## Traceability

Implementation Issues should reference applicable specification IDs, for example:

```text
RF-CRE-010
RNF-AUZ-001
RNF-CAP-001
ADR-0004
```

Requirements remain the source of truth even after implementation Issues are closed.
