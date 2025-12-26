using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.ServiceCenters.Models
{
    /// <summary>
    /// Simplified DTO for service center list view
    /// Contains only essential information for listing
    /// </summary>
    public sealed class ServiceCenterListDto
    {
        // <summary>
        /// Gets or sets the service center ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the service center name in English
        /// </summary>
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service center name in Arabic
        /// </summary>
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city name
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contact phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the working hours
        /// </summary>
        public string? WorkingHours { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the service center.
        /// Used by clients (e.g., maps) to place markers.
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the service center.
        /// Used by clients (e.g., maps) to place markers.
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the distance from user location (kilometers)
        /// Only populated when location filtering is used
        /// </summary>
        public double? Distance { get; set; }

        /// <summary>
        /// Gets or sets the number of services offered
        /// </summary>
        public int ServicesCount { get; set; }

        /// <summary>
        /// Gets or sets whether the service center is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}