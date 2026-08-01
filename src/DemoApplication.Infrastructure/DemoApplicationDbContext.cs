using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Infrastructure;

/// <summary>Owns the SQLite persistence model and its deterministic baseline seed data.</summary>
public sealed class DemoApplicationDbContext : DbContext
{
    /// <summary>Initializes the context with configured provider options.</summary>
    /// <param name="options">Provider and database options.</param>
    public DemoApplicationDbContext(DbContextOptions<DemoApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets the baseline roles.</summary>
    public DbSet<PersistenceRole> Roles => Set<PersistenceRole>();

    /// <summary>Gets the minimal order index records.</summary>
    public DbSet<OrderIndexRecord> Orders => Set<OrderIndexRecord>();

    /// <summary>Gets the minimal shipment index records.</summary>
    public DbSet<ShipmentIndexRecord> Shipments => Set<ShipmentIndexRecord>();

    /// <summary>Configures table names, indexes, keys, and deterministic seed records.</summary>
    /// <param name="modelBuilder">Entity model builder supplied by Entity Framework Core.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Keep the schema foundation independent from future business workflow entities.
        modelBuilder.Entity<PersistenceRole>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(role => role.Id);
            entity.Property(role => role.Name).IsRequired().HasMaxLength(64);
            entity.Property(role => role.NormalizedName).IsRequired().HasMaxLength(64);
            entity.HasIndex(role => role.NormalizedName).IsUnique();
            entity.HasData(
                new PersistenceRole { Id = new Guid("2e6bd8e6-1a43-4ec2-a2ef-6e9e2ef7f44c"), Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                new PersistenceRole { Id = new Guid("d9aa8d4e-969c-4ea8-9b11-1c3f4d3b35f2"), Name = "User", NormalizedName = "USER" });
        });

        modelBuilder.Entity<OrderIndexRecord>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(order => order.Id);
            entity.Property(order => order.Status).IsRequired().HasMaxLength(32);
            entity.HasIndex(order => new { order.Status, order.CreatedUtc });
        });

        modelBuilder.Entity<ShipmentIndexRecord>(entity =>
        {
            entity.ToTable("Shipments");
            entity.HasKey(shipment => shipment.Id);
            entity.Property(shipment => shipment.Status).IsRequired().HasMaxLength(32);
            entity.HasIndex(shipment => new { shipment.OrderId, shipment.Status });
        });
    }
}
