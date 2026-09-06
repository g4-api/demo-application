using Asp.Versioning.ApiExplorer;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApplication.Api.OpenApi
{
    /// <summary>
    /// Wires the request pipeline that serves the versioned OpenAPI documents and the Swagger UI.
    /// </summary>
    public static class ApiDocumentationApplicationBuilderExtensions
    {
        /// <summary>
        /// Serves the generated <c>swagger.json</c> for every API version and mounts the Swagger UI at <c>/swagger</c>.
        /// </summary>
        /// <remarks>
        /// Call before the fallback file mapping so the documentation routes are matched ahead of the SPA. The
        /// method reads the discovered versions from <see cref="IApiVersionDescriptionProvider"/> and registers
        /// one Swagger UI endpoint per version, newest first.
        /// </remarks>
        /// <param name="app">Configured web application whose pipeline receives the documentation middleware.</param>
        /// <returns>The same application for pipeline chaining.</returns>
        public static WebApplication UseApiDocumentation(this WebApplication app)
        {
            ArgumentNullException.ThrowIfNull(
                argument: app,
                paramName: nameof(app));

            // Resolve the version projection from the built container so the UI endpoint list matches the specs.
            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            // Emit the raw OpenAPI documents before mounting the UI that consumes them.
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                // Register newest-first so the UI opens on the current version rather than the oldest one.
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
                {
                    var documentUrl = $"/swagger/{description.GroupName}/swagger.json";
                    options.SwaggerEndpoint(documentUrl, $"Demo Application API {description.GroupName}");
                }
            });

            return app;
        }
    }
}
