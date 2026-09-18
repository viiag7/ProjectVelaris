# Agile Development

## Cadence

Velaris uses **two-week sprints** by default.

The cadence may be changed later if delivery data shows another interval works better.

## Sprint flow

### 1. Backlog Refinement

Before Sprint Planning:

- clarify the desired outcome;
- link applicable RF/RNF/ADR identifiers;
- define acceptance criteria;
- identify dependencies;
- identify security, reliability and observability implications;
- estimate the Story;
- create a Spike when uncertainty is too high to estimate responsibly.

### 2. Sprint Planning

Select only items that meet the Definition of Ready.

Define one clear **Sprint Goal**.

Do not fill a sprint based only on theoretical capacity. Reserve capacity for reviews, integration, defects and unexpected technical work.

### 3. During the Sprint

Work should move through:

```text
Backlog
  ↓
Ready
  ↓
In Progress
  ↓
In Review
  ↓
Done
```

Keep Work In Progress low.

When new urgent work enters the sprint, explicitly reconsider existing commitments rather than silently increasing scope.

### 4. Sprint Review

Demonstrate working outcomes rather than only reporting completed tasks.

For technical infrastructure work, evidence may include:

- running software;
- benchmark results;
- failure/recovery tests;
- security test results;
- dashboards and telemetry;
- ADR conclusions.

### 5. Retrospective

Review:

- what helped delivery;
- what slowed delivery;
- quality problems;
- escaped defects;
- process improvements.

Choose a small number of concrete improvements for the next sprint.

## Vertical slices

Prefer end-to-end slices over horizontal component completion.

Example:

```text
HTTP submission
  -> authentication
  -> sender authorization
  -> quota validation
  -> persistence
  -> queue
  -> worker
  -> observable result
```

This is preferred over completing all database tables first, then all APIs, then all workers.

## Technical Spikes

Use a Spike when an architectural or performance question cannot be answered responsibly from existing evidence.

Examples:

- broker topology benchmark;
- quota concurrency strategy;
- AuthZEN PDP integration;
- SMTP throughput per worker;
- MIME/object-storage strategy.

A Spike must be **time-boxed** and end with evidence.

Expected output is usually:

- test/prototype;
- measurements;
- conclusion;
- ADR or recommendation;
- follow-up Stories.

A Spike is not considered successful merely because prototype code was written.

## Critical-system rule

Security, reliability, backup/recovery, performance and observability are part of the implementation, not a later hardening phase.
