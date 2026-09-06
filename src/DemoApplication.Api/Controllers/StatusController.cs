using Asp.Versioning;

using DemoApplication.Api.Contracts;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Swashbuckle.AspNetCore.Annotations;

using System.Net.Mime;

namespace DemoApplication.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [SwaggerTag(description: "Reports whether an API host is running. Used by deployment smoke checks and uptime probes.")]
    public sealed class StatusController : ControllerBase
    {
        [HttpGet]
        #region *** Open API Documentation ***
        [SwaggerOperation(
            summary: "Get API status",
            description: "Returns the shared success envelope with a coarse health word and the API version that served the request.")]
        [SwaggerResponse(
            statusCode: StatusCodes.Status200OK,
            description: "The API host is running and able to serve requests.",
            type: typeof(ApiResponse<StatusReport>),
            contentTypes: MediaTypeNames.Application.Json)]
        #endregion
        public ActionResult<ApiResponse<StatusReport>> GetStatus()
        {
            // Read the version segment that routing matched so the payload echoes the caller's contract, not a constant.
            var routedVersion = HttpContext.GetRouteValue("version") as string;
            var versionLabel = string.IsNullOrWhiteSpace(routedVersion)
                ? "v1"
                : $"v{routedVersion}";

            // Wrap the fixed status word in the shared envelope so smoke checks read one response shape everywhere.
            var report = new StatusReport
            {
                Status = "ok",
                ApiVersion = versionLabel
            };

            return Ok(ApiResponse<StatusReport>.Success(report));
        }
    }
}
