namespace Data;

/// <summary>
/// Represents the current status of an order in the system
/// </summary>
public enum OrderStatus
{
    /// <summary>Order has been created but processing has not started</summary>
    Pending,

    /// <summary>Order is currently being prepared</summary>
    InProgress,

    /// <summary>Order has been fulfilled and delivered</summary>
    Completed,

    /// <summary>Order has been cancelled and will not be processed</summary>
    Cancelled
}