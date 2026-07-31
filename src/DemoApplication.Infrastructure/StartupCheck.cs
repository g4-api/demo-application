using DemoApplication.Application.Abstractions;

namespace DemoApplication.Infrastructure;

/// <summary>Provides the baseline startup check for infrastructure wiring.</summary>
public sealed class StartupCheck : IStartupCheck
{
    /// <summary>Completes successfully when baseline infrastructure is available.</summary>
    public Task CheckAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
