# API versioning and contract policy

This document defines how the Demo Application HTTP API is versioned, how the
contract is published, and what counts as a breaking change.

## Route strategy

- Every controller endpoint is served under `/api/v{major}`, for example
  `/api/v1/status`.
- The version travels in the URL path only. Query string and header version
  readers are not enabled.
- `v1` is the default version. A request that omits the segment does not fall
  back to a default; it must address a concrete version.
- Only the major version appears in the route. Minor revisions are additive and
  never change the path.
- Health probing stays outside the versioned surface. `GET /health` is
  infrastructure, not part of the API contract.
- Any unmatched route under `/api` returns `404` rather than the SPA shell.

## Version discovery

- Every response carries an `api-supported-versions` header, and a deprecated
  version also carries `api-deprecated-versions`.
- The OpenAPI document for a version is served at
  `/swagger/v{major}/swagger.json`.
- The Swagger UI is served at `/swagger` and lists every published version.

## Response and error contract

- Successful responses use the shared `ApiResponse<T>` envelope:
  `succeeded`, `data`, and `errors`.
- `errors` is always present. It is an empty array on success and carries
  human-readable messages for a rejected but well-formed request.
- Validation failures and unhandled faults are returned as RFC 7807 problem
  documents with the `application/problem+json` media type. The API does not
  wrap faults in the success envelope.
- Every operation in the OpenAPI document declares `400` and `500` problem
  responses in addition to its own responses.

## Authentication

- The OpenAPI document declares a single security scheme named `Bearer`: an
  HTTP bearer scheme carrying a JWT.
- The scheme is applied as a global requirement so future protected endpoints
  inherit it. Endpoint-level enforcement arrives with the authentication
  milestone; the current baseline endpoints remain anonymous.

## What counts as a breaking change

Introduce a new major version (`/api/v2`) for any of the following:

- Removing an endpoint, field, or enum value.
- Renaming a field, route, or query parameter.
- Changing the type or format of an existing field.
- Making a previously optional request field required.
- Changing the meaning of an existing field or status code.
- Changing default behavior in a way an existing client can observe.
- Tightening validation so a previously accepted request is rejected.

## What is allowed inside a major version

The following changes are additive and ship without a new version:

- Adding a new endpoint.
- Adding an optional request field.
- Adding a field to a response.
- Adding a new enum value that existing clients can ignore.
- Relaxing validation to accept more inputs.

## Adding a new version

1. Register the new `ApiVersion` on the affected controllers, keeping the
   previous version in place.
2. Mark the retiring version with `[Obsolete]` handling through
   `ApiVersion.Deprecated` so discovery headers and the OpenAPI description
   announce the sunset.
3. Publish both documents until the deprecated version is removed.
4. Announce the removal date before dropping the deprecated version.
