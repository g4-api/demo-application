using Microsoft.AspNetCore.Diagnostics;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

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
