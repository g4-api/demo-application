namespace DemoApplication.Infrastructure;

/// <summary>Stores the baseline role names required by the application.</summary>
public sealed class PersistenceRole
{
    /// <summary>Gets or sets the stable role identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the display name of the role.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the normalized role name used for case-insensitive lookup.</summary>
    public string NormalizedName { get; set; } = string.Empty;
}
