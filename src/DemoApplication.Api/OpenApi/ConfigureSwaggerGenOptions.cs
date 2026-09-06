using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace DemoApplication.Api.OpenApi
{
    /// <summary>
    /// Builds one Swagger document per discovered API version and declares the shared security and error contract.
    /// </summary>
    /// <remarks>
    /// Swashbuckle resolves this configurator from the container after the API explorer has enumerated the
    /// versioned endpoints, so <see cref="Configure(SwaggerGenOptions)"/> runs once per document generation and
    /// stays in sync with the versions registered by the versioning middleware.
    /// </remarks>
    public sealed class ConfigureSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
    {
        // Holds the explorer projection that lists every API version the versioning middleware knows about.
        private readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;

        /// <summary>
        /// Initializes the configurator with the API version projection owned by the versioning middleware.
        /// </summary>
        /// <param name="apiVersionDescriptionProvider">Projection of every registered API version.</param>
        public ConfigureSwaggerGenOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            ArgumentNullException.ThrowIfNull(
                argument: apiVersionDescriptionProvider,
                paramName: nameof(apiVersionDescriptionProvider));

            _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        }

        /// <summary>
        /// Registers a document for every API version and adds the JWT bearer scheme required by future write endpoints.
        /// </summary>
        /// <param name="options">Swagger generator options mutated in place before document generation.</param>
        public void Configure(SwaggerGenOptions options)
        {
            ArgumentNullException.ThrowIfNull(
                argument: options,
                paramName: nameof(options));

            // Expose the [Swagger*] annotation attributes so controllers can document the contract inline.
            options.EnableAnnotations();

            // Treat non-nullable reference types as required so the published schema matches the C# contract.
            options.SupportNonNullableReferenceTypes();

            // Publish the shared RFC 7807 error responses on every operation without per-action attributes.
            options.OperationFilter<ProblemDetailsOperationFilter>();

            // Emit one document per discovered version so a future v2 is published without touching this wiring.
            foreach (var description in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, NewVersionInfo(description));
            }

            // Declare the bearer scheme in the spec so the documented auth requirement is discoverable now,
            // even though enforcement arrives with the authentication milestone.
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT bearer token issued by the Demo Application identity endpoints. Format: 'Bearer {token}'.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };

            options.AddSecurityDefinition("Bearer", securityScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                [securityScheme] = []
            });
        }

        // Builds the per-version document header, marking retired versions so consumers can plan a migration.
        // The helper reads only the supplied description and has no side effects.
        private static OpenApiInfo NewVersionInfo(ApiVersionDescription description)
        {
            // Start from the contract summary shared by every version of the API.
            var descriptionText = "REST contract for the Demo Application logistics platform. " +
                "Successful responses use the shared ApiResponse envelope; failures use RFC 7807 ProblemDetails. " +
                "Versioning and breaking-change policy: docs/api-versioning.md.";

            // Append the sunset notice when the API explorer reports the version as deprecated.
            if (description.IsDeprecated)
            {
                descriptionText += " This API version is deprecated and will be removed in a future release.";
            }

            return new OpenApiInfo
            {
                Title = "Demo Application API",
                Version = description.ApiVersion.ToString(),
                Description = descriptionText
            };
        }
    }
}
