# GitHub Project Setup

This document defines the desired GitHub configuration for the Velaris agile workflow.

## Project fields

Create a GitHub Project with these fields:

### Status

```text
Backlog
Ready
In Progress
In Review
Done
```

### Type

```text
Epic
Story
Task
Spike
Bug
```

### Priority

```text
P0
P1
P2
P3
```

### Size

```text
1
2
3
5
8
```

### Iteration

Use two-week iterations.

## Recommended views

### Backlog

All open items grouped or sorted by Priority.

### Current Sprint

Filter to the active Iteration and group by Status.

### Roadmap

Display Epics grouped by Milestone.

### Technical Risk

Filter Type = Spike plus security/performance/reliability-related work.

## Recommended repository labels

Use labels mainly for stable classification, not workflow status.

### Type

```text
type:epic
type:story
type:task
type:spike
type:bug
```

### Area

```text
area:tenancy
area:auth
area:domains
area:credentials
area:submission
area:messaging
area:delivery
area:inbound
area:database
area:infrastructure
area:observability
area:security
area:documentation
```

Avoid labels such as `in-progress` or `done`; use the Project Status field instead.

## Milestones

Use GitHub Milestones for meaningful release/project outcomes, not individual sprints.

Examples of the intended level:

- Architecture Validation
- Production Core
- Production Readiness

Actual Milestones should be created only after their scope and exit criteria are agreed.

## Branch governance

The `main` branch should ultimately:

- require changes through Pull Requests;
- prohibit force pushes;
- require CI checks once CI exists;
- require conversations to be resolved;
- avoid bypass except for explicitly controlled emergency procedures.

When the project has multiple active contributors, require at least one independent review for production code.

## Issue linkage

Stories should be linked to their parent Epic.

Tasks should be linked to the Story they support.

Pull Requests should close or reference their implementation Issue.

## Automation opportunities

Once the Project is created, GitHub automation may:

- add newly created Issues to Backlog;
- move an item to In Progress when work starts;
- move to In Review when a PR is opened;
- move to Done when its Issue is closed.

Automation should support the process rather than hide state transitions that matter to the team.
