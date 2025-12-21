using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.Vehicles.Models
{
    /// <summary>
    /// Simplified DTO for vehicle list view
    /// Contains only essential information
    /// </summary>
    public sealed class VehicleListDto
    {
        /// <summary>
        /// Gets or sets the vehicle ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the vehicle brand
        /// </summary>
        public string Brand { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle model
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
        /// Gets or sets the vehicle color
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public string DisplayName => $"{Brand} {Model} ({Year})";
    }
}