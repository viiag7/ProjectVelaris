# .NET Code Practices

This document defines the coding practices for Velaris .NET code. Its purpose is to keep the codebase understandable by humans, safe to change and inexpensive to maintain.

These are engineering guidelines, not product requirements. Apply them with judgment, but make exceptions explicit during code review.

## Primary rule

Write code for the next developer who must understand, operate and safely change it.

Prefer, in this order:

1. correctness and security;
2. clarity;
3. simplicity;
4. testability;
5. measured performance.

Do not trade readability for speculative optimization or cleverness. Optimize only after measurement identifies a meaningful bottleneck.

## Classes and responsibilities

A class should have one clear reason to change and a name that describes that responsibility.

- Keep domain rules separate from protocol, persistence and provider SDK code.
- Prefer composition over inheritance.
- Depend on small, purpose-specific interfaces at architectural boundaries.
- Do not introduce an interface for every class when there is no boundary, alternate implementation or testing value.
- Do not use static service locators, global mutable state or hidden dependencies.
- Constructor dependencies must represent real collaborators. A long dependency list usually means the class owns too many responsibilities.
- Do not split a large class with `partial` or `#region` merely to hide its size. Extract cohesive behavior instead.
- Prefer one primary public type per file so names and locations remain predictable.

There is no universal line-count rule, but the following are review signals:

- a class approaching 300 lines;
- a method approaching 40 lines;
- a constructor with more than 5 dependencies;
- nesting deeper than 3 levels;
- repeated changes to unrelated parts of the same class;
- tests that require extensive setup to exercise one behavior.

Crossing a signal is not automatically wrong. It requires the author and reviewer to confirm that the code is still cohesive and easier to understand than the available refactoring.

## Methods and control flow

- Give each method one understandable purpose.
- Use names that describe intent, not implementation mechanics.
- Prefer guard clauses over deeply nested conditionals.
- Keep happy paths visible and failure paths explicit.
- Avoid boolean parameters whose meaning is unclear at the call site. Prefer an enum, options type or separate operation.
- Avoid methods with long parameter lists. Group values only when they form a meaningful concept.
- Remove duplication when the shared behavior and its reason to change are genuinely the same.
- Do not create abstractions merely because two small blocks currently look similar.
- Make side effects visible in method names and boundaries.

## Naming and domain language

- Use the terminology defined by Velaris: Tenant, Environment, Credential, Sender Grant, Message, Delivery, Suppression and Attachment.
- Prefer complete, searchable names over abbreviations.
- Use verbs for operations and nouns for values or entities.
- Boolean names should read naturally, such as `IsActive`, `HasRecipients` or `CanAccept`.
- Include units in names where ambiguity is possible, such as `TimeoutSeconds` or `SizeBytes`.
- Do not use generic names such as `Manager`, `Helper`, `Processor`, `Utils` or `Data` unless the responsibility is genuinely precise from the surrounding context.

## Domain and architecture boundaries

Dependencies point inward:

```text
Host / SMTP / Persistence / Storage
                ↓
           Application
                ↓
              Domain
```

- Domain code must not depend on Entity Framework Core, Npgsql, SMTP libraries, Object Storage SDKs or OpenTelemetry.
- Application use cases coordinate domain behavior and ports; they do not contain provider-specific details.
- Infrastructure adapters translate external models and failures into application contracts.
- Do not pass EF entities, SDK models or SMTP-library types across application boundaries.
- Keep transaction ownership explicit at the use-case boundary.
- Avoid a generic repository that only renames `DbSet` operations without enforcing a useful boundary or invariant.

## C# and .NET defaults

- Enable nullable reference types.
- Treat compiler and analyzer warnings as defects; suppress a warning only with a documented reason.
- Prefer immutable values and `record` types when value semantics are intended.
- Use `required` members or constructors to prevent invalid initialization.
- Use `CancellationToken` for asynchronous I/O and propagate it through the complete call chain.
- Use the asynchronous APIs of network, database and storage dependencies. Do not block on tasks with `.Result`, `.Wait()` or `GetAwaiter().GetResult()` in application code.
- Use `TimeProvider` or an application clock abstraction for behavior controlled by time.
- Validate configuration during startup with strongly typed options.
- Use `IAsyncDisposable` and `await using` for asynchronous resources where applicable.
- Do not implement cryptographic primitives. Use reviewed platform or established library implementations.

## Error handling

- Model expected business or policy failures explicitly; do not use exceptions for ordinary control flow.
- Use exceptions for unexpected or infrastructure failures and preserve the original cause.
- Catch an exception only when the current layer can recover, add useful context or translate it at a boundary.
- Never use an empty `catch` or continue after an unknown failure as if the operation succeeded.
- Centralize translation from application failures to SMTP replies.
- Retry only failures explicitly classified as transient and safe to repeat.
- Do not expose database, storage, Tenant or Credential internals in client-facing error messages.

## Entity Framework Core

- Keep the `DbContext` lifetime aligned with one application operation or explicit unit of work.
- Make transaction boundaries visible in the application/persistence collaboration.
- Use `AsNoTracking` for read-only queries unless tracking is intentionally required.
- Project only the fields needed by a query when loading a full aggregate is unnecessary.
- Avoid lazy loading and hidden N+1 query behavior.
- Inspect generated SQL for security-sensitive, concurrency-sensitive or high-volume paths.
- Use reviewed parameterized SQL through EF Core/Npgsql when LINQ cannot express the required conditional or set-based operation safely.
- Never concatenate untrusted values into SQL.
- Keep migrations small, review their generated SQL and test them against real PostgreSQL.
- Do not hide concurrency or quota invariants behind a generic data-access abstraction.

## Logging and observability

- Use structured logging with stable event names and properties.
- Log enough context to diagnose behavior, but never log secrets, SCRAM proofs/verifiers, message bodies or attachment content.
- Do not interpolate structured values into the log message template.
- Preserve correlation between the SMTP session, authentication, Tenant/Environment, Message and Deliveries using approved identifiers.
- Avoid unbounded high-cardinality metric labels.
- An error log must help an operator understand what failed and what action is possible.

## Tests as readable specifications

- Test externally observable behavior and invariants rather than private implementation details.
- Give tests names that describe the condition and expected outcome.
- Keep Arrange, Act and Assert visually clear without requiring comments for obvious steps.
- Prefer small hand-written fakes at application boundaries over large, behavior-heavy mocking setups.
- Use real PostgreSQL and storage-compatible containers for persistence and adapter integration tests.
- Add regression tests before fixing a defect when practical.
- Cover failure, cancellation, timeout, concurrency and Tenant-isolation paths, not only the happy path.
- A difficult-to-test class is a design signal; simplify its responsibilities before adding more mocking infrastructure.

## Comments and documentation

- Prefer code that explains **what** it does through names and structure.
- Use comments to explain **why** a non-obvious constraint or trade-off exists.
- Remove stale or redundant comments during the same change that makes them inaccurate.
- Add XML documentation to public contracts where consumers need semantic, failure or lifecycle information.
- Link protocol, security or database decisions to the relevant RF, RNF, ADR or RFC when that context prevents unsafe changes.
- Do not leave commented-out code; version control already preserves history.

## Refactoring expectations

Refactor before extending code when:

- a change would add another unrelated responsibility to a class;
- the same policy is implemented in more than one place;
- a method mixes protocol parsing, business policy and persistence;
- a class cannot be named without words such as “and” or “everything”;
- a test must know many internal details to verify one outcome;
- a new conditional makes an already complex path harder to reason about.

Refactoring must preserve behavior and remain covered by tests. Large rewrites require a demonstrated benefit and an incremental migration path.

## Pull-request review checklist

- [ ] Can a reviewer explain the change without reverse-engineering clever code?
- [ ] Do classes and methods have one clear responsibility?
- [ ] Are names consistent with the Velaris domain language?
- [ ] Are dependencies and side effects explicit?
- [ ] Are protocol, application, domain and infrastructure concerns separated?
- [ ] Is asynchronous work cancellable without sync-over-async calls?
- [ ] Are errors translated at the correct boundary and retries narrowly classified?
- [ ] Are EF Core queries and transaction boundaries understandable?
- [ ] Are security-sensitive values excluded from logs and telemetry?
- [ ] Do tests cover the important behavior and failure paths?
- [ ] Did the change remove obsolete code, comments and accidental complexity?
- [ ] Would a developer unfamiliar with the change be able to maintain it safely?

## Definition of maintainable code

Code is maintainable when a developer can:

- find the relevant behavior quickly;
- understand its inputs, outputs, dependencies and side effects;
- change one responsibility without unexpectedly changing another;
- verify the change with focused automated tests;
- diagnose failures through safe logs, metrics and traces;
- explain why the implementation exists and which requirements or decisions govern it.

If the code cannot meet those conditions, it is not finished even when it compiles.
