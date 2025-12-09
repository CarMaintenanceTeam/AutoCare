namespace AutoCare.Domain.Enums
{
    /// <summary>
/// Represents the type of user in the system
/// </summary>
    public enum UserType
    {
        /// <summary>
    /// Regular customer who books services
    /// </summary>
    Customer = 1,

    /// <summary>
    /// Service center employee
    /// </summary>
    Employee = 2,

    /// <summary>
    /// System administrator
    /// </summary>
    Admin = 3
    }
}