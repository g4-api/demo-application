using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Api.Controllers;

/// <summary>Exposes the API status contract used by smoke checks.</summary>
[ApiController]
[Route("api/status")]
public sealed class StatusController : ControllerBase
{
    /// <summary>Returns a stable response envelope for a running API.</summary>
    [HttpGet]
    public ActionResult<ApiResponse<object>> GetStatus()
    {
        return Ok(ApiResponse<object>.Success(new { status = "ok" }));
    }
}

/// <summary>Represents a consistent API response envelope.</summary>
public sealed record ApiResponse<T>(bool Succeeded, T? Data, IReadOnlyCollection<string> Errors)
{
    /// <summary>Creates a successful response.</summary>
    public static ApiResponse<T> Success(T data) => new(true, data, Array.Empty<string>());
}
