using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Service to get current authenticated user information
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// Gets the current user ID (from JWT claims)
        /// </summary>
        int? UserId { get; }

        /// <summary>
        /// Gets the current user email
        /// </summary>
        string? Email { get; }

        /// <summary>
        /// Gets the current user type (Customer, Employee, Admin)
        /// </summary>
        string? UserType { get; }

        /// <summary>
        /// Checks if user is authenticated
        /// </summary>
        bool IsAuthenticated();

        /// <summary>
        /// Checks if user is in specified role
        /// </summary>
        bool IsInRole(string role);
    }
}