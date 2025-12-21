using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.Services.Models
{
    /// <summary>
    /// Simplified DTO for service list view
    /// Contains only essential information
    /// </summary>
    public sealed class ServiceListDto
    {
        /// <summary>
        /// Gets or sets the service ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the service name in English
        /// </summary>
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service name in Arabic
        /// </summary>
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base price
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Gets or sets the estimated duration in minutes
        /// </summary>
        public int EstimatedDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the service type
        /// </summary>
        public string ServiceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the service is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the number of service centers offering this service
        /// </summary>
        public int AvailableAt { get; set; }
    }
}