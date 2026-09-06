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

The API listens on the URL printed by the host. Verify startup with `GET /health` and `GET /api/v1/status`.

Open `/swagger` for the interactive OpenAPI UI.

## API conventions

The full policy, including breaking-change rules, lives in
[docs/api-versioning.md](docs/api-versioning.md). The essentials:

- **Versioning.** Every endpoint is served under `/api/v{major}` (currently
  `/api/v1`). The version is a required URL segment; only the major version
  appears in the route. `GET /health` stays outside the versioned surface.
- **Discovery.** Responses carry an `api-supported-versions` header. Per-version
  OpenAPI documents are served at `/swagger/v{major}/swagger.json` and listed in
  the Swagger UI at `/swagger`.
- **Success envelope.** Successful responses use `ApiResponse<T>`:

  ```json
  {
    "succeeded": true,
    "data": { "status": "ok", "apiVersion": "v1" },
    "errors": []
  }
  ```

- **Error schema.** Validation failures and unhandled faults return RFC 7807
  problem documents (`application/problem+json`), never the success envelope.
  Every operation documents `400` and `500` problem responses.
- **Authentication.** The OpenAPI document declares a global `Bearer` (JWT)
  security scheme. Enforcement lands with the authentication milestone; the
  current baseline endpoints are anonymous.
- **Breaking changes.** Removing or renaming a field, changing a type, or
  tightening validation requires a new major version. Additive changes ship
  within the current version. See the policy document for the full list.
