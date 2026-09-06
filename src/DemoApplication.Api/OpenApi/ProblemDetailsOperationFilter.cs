using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace DemoApplication.Api.OpenApi
{
    /// <summary>
    /// Documents the shared RFC 7807 error contract on every operation that does not already declare it.
    /// </summary>
    /// <remarks>
    /// The API returns <see cref="ProblemDetails"/> for validation failures and <c>ProblemDetails</c> for unhandled
    /// faults through the shared exception handler. Declaring the responses centrally keeps controllers free of
    /// repeated error attributes while still publishing a complete contract.
    /// </remarks>
    public sealed class ProblemDetailsOperationFilter : IOperationFilter
    {
        // Status codes the shared pipeline can return for any operation regardless of the action body.
        private static readonly IReadOnlyDictionary<string, string> BaselineErrorResponses = new Dictionary<string, string>
        {
            [StatusCodes.Status400BadRequest.ToString()] = "The request failed validation. The body is an RFC 7807 problem document.",
            [StatusCodes.Status500InternalServerError.ToString()] = "The API failed to process the request. The body is an RFC 7807 problem document."
        };

        /// <summary>
        /// Adds the baseline problem responses to <paramref name="operation"/> when the action omits them.
        /// </summary>
        /// <param name="operation">OpenAPI operation being generated for a single action.</param>
        /// <param name="context">Generation context for the current action.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            ArgumentNullException.ThrowIfNull(
                argument: operation,
                paramName: nameof(operation));

            ArgumentNullException.ThrowIfNull(
                argument: context,
                paramName: nameof(context));

            // Register the ProblemDetails schema once so every added response can reference it.
            var problemDetailsSchema = context.SchemaGenerator.GenerateSchema(
                modelType: typeof(ProblemDetails),
                schemaRepository: context.SchemaRepository);

            // Fill each baseline status code the action has not already documented itself.
            foreach (var errorResponse in BaselineErrorResponses)
            {
                if (operation.Responses.ContainsKey(errorResponse.Key))
                {
                    continue;
                }

                operation.Responses[errorResponse.Key] = new OpenApiResponse
                {
                    Description = errorResponse.Value,
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/problem+json"] = new OpenApiMediaType
                        {
                            Schema = problemDetailsSchema
                        }
                    }
                };
            }
        }
    }
}
