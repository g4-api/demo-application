namespace DemoApplication.Infrastructure;

/// <summary>Provides the minimum order shape required by persistence query indexes.</summary>
public sealed class OrderIndexRecord
{
    /// <summary>Gets or sets the order identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the order status used by indexed queries.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the creation timestamp used by indexed queries.</summary>
    public DateTime CreatedUtc { get; set; }
}

/// <summary>Provides the minimum shipment shape required by persistence query indexes.</summary>
public sealed class ShipmentIndexRecord
{
    /// <summary>Gets or sets the shipment identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the related order identifier.</summary>
    public Guid OrderId { get; set; }

    /// <summary>Gets or sets the shipment status used by indexed queries.</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the update timestamp used by indexed queries.</summary>
    public DateTime UpdatedUtc { get; set; }
}
