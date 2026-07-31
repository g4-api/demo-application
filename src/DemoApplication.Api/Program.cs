using DemoApplication.Application.Abstractions;
using DemoApplication.Api;
using DemoApplication.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration, builder.Environment.ContentRootPath);
builder.Services.AddSingleton<IStartupCheck, StartupCheck>();
builder.Services.AddHostedService<StartupValidationHostedService>();

WebApplication app = builder.Build();
app.UseExceptionHandler();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHealthChecks("/health");
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

public partial class Program
{
}
