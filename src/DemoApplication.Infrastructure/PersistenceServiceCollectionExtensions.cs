using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoApplication.Infrastructure;

/// <summary>Registers the SQLite persistence foundation in the application host.</summary>
public static class PersistenceServiceCollectionExtensions
{
    /// <summary>Registers the configurable SQLite context factory.</summary>
    /// <param name="services">Application service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    /// <param name="contentRootPath">Host content root used to resolve relative paths.</param>
    /// <returns>The same service collection for composition chaining.</returns>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, string contentRootPath)
    {
        // Validate the composition inputs before resolving paths or mutating the service collection.
        ArgumentNullException.ThrowIfNull(
            argument: configuration,
            paramName: nameof(configuration));

        ArgumentException.ThrowIfNullOrWhiteSpace(
            argument: contentRootPath,
            paramName: nameof(contentRootPath));

        // Resolve the configured path once so relative and absolute locations share the same provider setup.
        var options = new PersistenceOptions
        {
            DatabasePath = configuration["Persistence:DatabasePath"] ?? "App_Data/demo-application.db"
        };
        var databasePath = Path.IsPathRooted(options.DatabasePath)
            ? options.DatabasePath
            : Path.Combine(contentRootPath, options.DatabasePath);
        var databaseDirectory = Path.GetDirectoryName(databasePath);

        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            // Ensure the configured parent exists before SQLite opens the file.
            Directory.CreateDirectory(databaseDirectory);
        }

        // Register a factory so startup and future request scopes receive independently owned contexts.
        services.AddDbContextFactory<DemoApplicationDbContext>(dbContextOptions => dbContextOptions.UseSqlite($"Data Source={databasePath}"));
        return services;
    }
}
