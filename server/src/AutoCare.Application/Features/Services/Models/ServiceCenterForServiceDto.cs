using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.Services.Models
{
    /// <summary>
    /// Data Transfer Object for service center information within service context
    /// Used when listing service centers that offer a specific service
    /// </summary>
    public sealed class ServiceCenterForServiceDto
    {
        /// <summary>
        /// Gets or sets the service center ID
        /// </summary>
        public int ServiceCenterId { get; set; }

        /// <summary>
        /// Gets or sets the service center name in English
        /// </summary>
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service center name in Arabic
        /// </summary>
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contact phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price at this specific service center
        /// Custom price if set, otherwise base price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets whether the service is currently available at this center
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets the distance from user location (if provided)
        /// </summary>
        public double? Distance { get; set; }
    }
}