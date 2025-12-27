using System;

namespace AutoCare.Application.Features.Customers.Models
{
    /// <summary>
    /// Simplified DTO for admin customer list view.
    /// </summary>
    public sealed class CustomerListDto
    {
        public int Id { get; set; }           // CustomerId
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public int VehiclesCount { get; set; }
        public int BookingsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}