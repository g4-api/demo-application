# Demo Application

API-first ASP.NET Core solution targeting .NET 10.

## Architecture

- `DemoApplication.Api` owns HTTP endpoints, middleware, health checks, and static SPA hosting.
- `DemoApplication.Application` owns use-case contracts and application orchestration.
- `DemoApplication.Domain` owns business entities and rules; it has no framework dependencies.
- `DemoApplication.Infrastructure` owns external integrations and implements Application abstractions.

Dependencies point inward: API references Application and Infrastructure; Infrastructure references Application and Domain; Application references Domain.

## Run

From the repository root on Windows, Linux, or macOS:

```bash
dotnet restore src/DemoApplication.slnx
dotnet run --project src/DemoApplication.Api
```

The API listens on the URL printed by the host. Verify startup with `GET /health` and `GET /api/status`.
