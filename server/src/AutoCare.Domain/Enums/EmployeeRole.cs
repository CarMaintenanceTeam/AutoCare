namespace AutoCare.Domain.Enums
{
    /// <summary>
/// Represents the role of an employee within a service center
/// </summary>
    public enum EmployeeRole
    {
     /// <summary>
    /// Technician who performs the actual service work
    /// Can update booking status
    /// </summary>
    Technician = 1,

    /// <summary>
    /// Manager who oversees operations
    /// Can confirm/reject bookings and manage technicians
    /// </summary>
    Manager = 2,

    /// <summary>
    /// Owner of the service center
    /// Has full access to all features and reports
    /// </summary>
    Owner = 3   
    }
}