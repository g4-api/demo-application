namespace DemoApplication.Infrastructure;

/// <summary>Defines configurable storage locations for application persistence.</summary>
public sealed class PersistenceOptions
{
    /// <summary>Gets or sets the SQLite database path relative to the content root or as an absolute path.</summary>
    public string DatabasePath { get; set; } = "App_Data/demo-application.db";
}
