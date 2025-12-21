using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;

namespace AutoCare.Application.Features.Services.Models
{
    /// <summary>
    /// Data Transfer Object for Service
    /// Contains complete service information
    /// </summary>
    public sealed class ServiceDto : AuditableEntityDto<int>
    {
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
        /// Gets or sets the base price for this service
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
        /// Gets or sets whether the service is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the list of service centers offering this service
        /// Only populated when explicitly requested
        /// </summary>
        public List<ServiceCenterForServiceDto>? ServiceCenters { get; set; }
    }
}