namespace Data;

/// <summary>
/// Represents the current status of an item in the inventory
/// </summary>
public enum ItemStatus
{
    /// <summary>Item is in stock and available for purchase</summary>
    Available,

    /// <summary>Item is temporarily out of stock</summary>
    OutOfStock,

    /// <summary>Item is no longer offered and won't be restocked</summary>
    Discontinued,

    /// <summary>Item is only available during specific seasons or time periods</summary>
    Seasonal
}