# Project Velaris

Velaris is a multi-tenant email delivery platform focused on reliable outbound email submission and SMTP delivery.

## Core model

```text
Platform
└── Tenant
    └── Environment
        ├── Domains
        ├── Credentials
        ├── Messages
        ├── Deliveries
        └── Suppressions
```

## Documentation

- [Documentation index](docs/README.md)
- [Product overview](docs/product/overview.md)
- [Terminology](docs/product/terminology.md)
- [Scope](docs/product/scope.md)
- [Functional requirements](docs/requirements/README.md)
- [Architecture](docs/architecture/overview.md)
- [Architecture Decision Records](docs/adr/README.md)
- [Future scope](docs/future/)

> Project status: specification / early design.
