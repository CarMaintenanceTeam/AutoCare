using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.ServiceCenters.Models
{
    /// <summary>
    /// Data Transfer Object for services offered at a service center
    /// Contains service details with custom pricing
    /// </summary>
    public sealed class ServiceCenterServiceDto
    {
        /// <summary>
        /// Gets or sets the service ID
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the service name in English
        /// </summary>
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service name in Arabic
        /// </summary>
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service description in English
        /// </summary>
        public string? DescriptionEn { get; set; }

        /// <summary>
        /// Gets or sets the service description in Arabic
        /// </summary>
        public string? DescriptionAr { get; set; }

        /// <summary>
        /// Gets or sets the effective price at this service center
        /// Custom price if set, otherwise base price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the base price of the service
        /// </summary>
        public decimal BasePrice { get; set; }

        /// <summary>
        /// Gets or sets the estimated duration in minutes
        /// </summary>
        public int EstimatedDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the service type (Maintenance, SpareParts)
        /// </summary>
        public string ServiceType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the service is available at this center
        /// </summary>
        public bool IsAvailable { get; set; }
    }
}