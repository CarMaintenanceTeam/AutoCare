using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.Authentication.Models
{
    /// <summary>
    /// Data Transfer Object for user information
    /// Excludes sensitive data like password hash
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the user ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's full name
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the user type (Customer, Employee, Admin)
        /// </summary>
        public string UserType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the user account is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets when the user account was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}