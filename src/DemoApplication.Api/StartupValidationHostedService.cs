using DemoApplication.Application.Abstractions;

namespace DemoApplication.Api
{

/// <summary>
/// Runs registered startup checks during host startup so unavailable infrastructure prevents serving requests.
/// </summary>
public sealed class StartupValidationHostedService : IHostedService
{
    private readonly IReadOnlyCollection<IStartupCheck> _startupChecks;

    /// <summary>
    /// Initializes the hosted validator with the checks owned by the application composition root.
    /// </summary>
    /// <param name="startupChecks">Registered checks that validate required startup dependencies.</param>
    public StartupValidationHostedService(IEnumerable<IStartupCheck> startupChecks)
    {
        ArgumentNullException.ThrowIfNull(
            argument: startupChecks,
            paramName: nameof(startupChecks));

        _startupChecks = startupChecks.ToArray();
    }

    /// <summary>
    /// Executes every startup check before the host begins accepting requests.
    /// </summary>
    /// <param name="cancellationToken">Token that cancels startup validation.</param>
    /// <returns>A task that completes after all checks pass.</returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Run each dependency check before the server starts so a failure prevents partial availability.
        foreach (var startupCheck in _startupChecks)
        {
            // Await the check with the host cancellation token so shutdown interrupts pending validation.
            await startupCheck.CheckAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Completes hosted-service shutdown without retaining startup resources.
    /// </summary>
    /// <param name="cancellationToken">Token supplied by the host during shutdown.</param>
    /// <returns>A completed task because this validator owns no shutdown work.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
}
