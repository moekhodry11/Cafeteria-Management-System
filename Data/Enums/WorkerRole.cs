namespace Data;

/// <summary>
/// Represents the role of a worker in the cafeteria system
/// </summary>
public enum WorkerRole
{
    /// <summary>Handles customer orders and payments</summary>
    Cashier,

    /// <summary>Oversees operations and has extended privileges</summary>
    Manager,

    /// <summary>Has full system access and configuration capabilities</summary>
    Admin,

    /// <summary>Responsible for food preparation and kitchen management</summary>
    Chef
}