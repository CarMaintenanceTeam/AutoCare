using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;

namespace AutoCare.Application.Features.ServiceCenters.Models
{
    /// <summary>
    /// Data Transfer Object for ServiceCenter
    /// Contains all service center information including services offered
    /// </summary>
    public class ServiceCenterDto : AuditableEntityDto<int>
    {
        /// <summary>
        /// Gets or sets the service center name in English
        /// </summary>
        public string NameEn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service center name in Arabic
        /// </summary>
        public string NameAr { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address in English
        /// </summary>
        public string AddressEn { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address in Arabic
        /// </summary>
        public string AddressAr { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city name
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the GPS latitude
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Gets or sets the GPS longitude
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Gets or sets the contact phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contact email address
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the working hours
        /// </summary>
        public string? WorkingHours { get; set; }

        /// <summary>
        /// Gets or sets whether the service center is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the distance from user location (kilometers)
        /// Only populated when location filtering is used
        /// </summary>
        public double? Distance { get; set; }

        /// <summary>
        /// Gets or sets the collection of services offered at this center
        /// </summary>
        public List<ServiceCenterServiceDto> Services { get; set; } = [];
    }
}