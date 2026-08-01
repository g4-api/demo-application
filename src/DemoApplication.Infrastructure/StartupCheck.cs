using DemoApplication.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Infrastructure;

/// <summary>Provides the baseline startup check for infrastructure wiring.</summary>
public sealed class StartupCheck : IStartupCheck
{
    // Holds the factory that owns short-lived contexts for startup migration work.
    private readonly IDbContextFactory<DemoApplicationDbContext> _dbContextFactory;

    /// <summary>Initializes the persistence startup check.</summary>
    /// <param name="dbContextFactory">Factory for the isolated startup migration context.</param>
    public StartupCheck(IDbContextFactory<DemoApplicationDbContext> dbContextFactory)
    {
        ArgumentNullException.ThrowIfNull(
            argument: dbContextFactory,
            paramName: nameof(dbContextFactory));

        _dbContextFactory = dbContextFactory;
    }

    /// <summary>Completes successfully when baseline infrastructure is available.</summary>
    public async Task CheckAsync(CancellationToken cancellationToken)
    {
        // Create a short-lived context so migration state is isolated from future request work.
        await using DemoApplicationDbContext dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken).ConfigureAwait(false);

        // Apply pending migrations before the host accepts traffic; seeded roles remain idempotent.
        await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
    }
}
