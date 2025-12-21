using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;

namespace AutoCare.Application.Features.Vehicles.Models
{
    /// <summary>
    /// Data Transfer Object for Vehicle
    /// Contains complete vehicle information
    /// </summary>
    public sealed class VehicleDto : AuditableEntityDto<int>
    {
        /// <summary>
        /// Gets or sets the customer ID who owns this vehicle
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the vehicle brand (e.g., Toyota, BMW)
        /// </summary>
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle model (e.g., Corolla, X5)
        /// </summary>
        public string Model { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manufacturing year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Gets or sets the license plate number
        /// </summary>
        public string PlateNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Vehicle Identification Number (VIN)
        /// </summary>
        public string? VIN { get; set; }

        /// <summary>
        /// Gets or sets the vehicle color
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the display name (Brand Model Year)
        /// Computed property for convenience
        /// </summary>
        public string DisplayName => $"{Brand} {Model} ({Year})";

        /// <summary>
        /// Gets or sets the number of bookings for this vehicle
        /// Only populated when explicitly requested
        /// </summary>
        public int? BookingsCount { get; set; }
    }
}