# Velaris Documentation

This directory is the source of truth for product requirements, architecture and technical decisions.

## Sections

- [Product](product/overview.md): goals, terminology and scope.
- [Requirements](requirements/README.md): functional requirements grouped by domain.
- [Architecture](architecture/overview.md): system model and message lifecycle.
- [ADRs](adr/README.md): important architecture decisions and their rationale.
- [Future](future/): features intentionally deferred from the initial scope.

## Documentation workflow

1. Discuss the desired behavior.
2. Update the relevant requirement or ADR.
3. Review the change in a pull request.
4. Merge the specification into `main`.
5. Create implementation issues referencing the requirement IDs.

GitHub Issues track implementation work. They do not replace functional requirements.
