using DemoApplication.Application.Abstractions;
using DemoApplication.Api;
using DemoApplication.Api.OpenApi;
using DemoApplication.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddApiVersioningAndDocumentation();
builder.Services.AddPersistence(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services.AddSingleton<IStartupCheck, StartupCheck>();
builder.Services.AddHostedService<StartupValidationHostedService>();

WebApplication app = builder.Build();
app.UseExceptionHandler();
app.UseApiDocumentation();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHealthChecks("/health");
app.MapControllers();

// Keep the SPA fallback off the API surface so an unknown route under /api returns a 404 contract
// instead of the HTML shell, and an unsupported version still reaches the versioning middleware.
app.MapFallback("/api/{**slug}", () => Results.NotFound());
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program
{
}
