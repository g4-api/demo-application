using Asp.Versioning;

using Microsoft.Extensions.DependencyInjection;

namespace DemoApplication.Api.OpenApi
{
    /// <summary>
    /// Registers the API versioning policy and the versioned OpenAPI document pipeline in the application host.
    /// </summary>
    public static class ApiDocumentationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds URL-segment API versioning and one Swagger document per version to the service collection.
        /// </summary>
        /// <remarks>
        /// Call after <c>AddControllers</c>. The method fixes <c>v1.0</c> as the default version, reports the
        /// supported and deprecated versions on every response, and wires <see cref="ConfigureSwaggerGenOptions"/>
        /// so document generation follows the versions known to the versioning middleware.
        /// </remarks>
        /// <param name="services">Application service collection.</param>
        /// <returns>The same service collection for composition chaining.</returns>
        public static IServiceCollection AddApiVersioningAndDocumentation(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(
                argument: services,
                paramName: nameof(services));

            // Pin the versioning contract: the major version travels in the URL segment, and every response
            // advertises the supported and deprecated versions so clients can detect a new major without a redeploy.
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddMvc()
                .AddApiExplorer(options =>
                {
                    // Produce group names such as 'v1' and rewrite the '{version}' route token in the published spec.
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            // Register the OpenAPI generator and defer document configuration until the version list is known.
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.ConfigureOptions<ConfigureSwaggerGenOptions>();

            return services;
        }
    }
}
