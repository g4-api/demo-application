namespace DemoApplication.Application.Abstractions;

/// <summary>Defines an application startup validation check.</summary>
public interface IStartupCheck
{
    /// <summary>Validates required application dependencies.</summary>
    Task CheckAsync(CancellationToken cancellationToken);
}
