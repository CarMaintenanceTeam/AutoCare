using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.Enums;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Service entity - represents maintenance services offered
    /// </summary>
    public sealed class Service : AuditableEntity<int>
    {
        #region Properties
        /// <summary>
        /// Gets the service name in English
        /// </summary>
        public string NameEn { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the service name in Arabic
        /// </summary>
        public string NameAr { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the service description in English (optional)
        /// </summary>
        public string? DescriptionEn { get; private set; }

        /// <summary>
        /// Gets the service description in Arabic (optional)
        /// </summary>
        public string? DescriptionAr { get; private set; }

        /// <summary>
        /// Gets the base price for this service
        /// </summary>
        public decimal BasePrice { get; private set; }

        /// <summary>
        /// Gets the estimated duration in minutes
        /// </summary>
        public int EstimatedDurationMinutes { get; private set; }

        /// <summary>
        /// Gets the service type (Maintenance or SpareParts)
        /// </summary>
        public ServiceType ServiceType { get; private set; }

        /// <summary>
        /// Gets whether the service is active and available for booking
        /// </summary>
        public bool IsActive { get; private set; } = true;

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the bookings for this service
        /// </summary>
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

        /// <summary>
        /// Gets the service centers offering this service (many-to-many)
        /// </summary>
        public ICollection<ServiceCenterService> ServiceCenterServices { get; private set; } = new List<ServiceCenterService>();

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private Service()
        {
        }

        #endregion

        #region Factory Methods
        /// <summary>
        /// Creates a new service
        /// </summary>
        /// <param name="nameEn">Name in English</param>
        /// <param name="nameAr">Name in Arabic</param>
        /// <param name="basePrice">Base price</param>
        /// <param name="estimatedDurationMinutes">Estimated duration in minutes</param>
        /// <param name="serviceType">Type of service</param>
        /// <param name="descriptionEn">Description in English (optional)</param>
        /// <param name="descriptionAr">Description in Arabic (optional)</param>
        /// <returns>A new Service entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static Service Create(
            string nameEn,
            string nameAr,
            decimal basePrice,
            int estimatedDurationMinutes,
            ServiceType serviceType,
            string? descriptionEn = null,
            string? descriptionAr = null)
        {
            // Validate names
            ValidateName(nameEn, "English");
            ValidateName(nameAr, "Arabic");

            // Validate price
            ValidatePrice(basePrice);

            // Validate duration
            ValidateDuration(estimatedDurationMinutes);

            var service = new Service
            {
                NameEn = nameEn.Trim(),
                NameAr = nameAr.Trim(),
                DescriptionEn = descriptionEn?.Trim(),
                DescriptionAr = descriptionAr?.Trim(),
                BasePrice = basePrice,
                EstimatedDurationMinutes = estimatedDurationMinutes,
                ServiceType = serviceType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return service;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Updates service names
        /// </summary>
        /// <param name="nameEn">New name in English</param>
        /// <param name="nameAr">New name in Arabic</param>
        public void UpdateNames(string nameEn, string nameAr)
        {
            ValidateName(nameEn, "English");
            ValidateName(nameAr, "Arabic");

            NameEn = nameEn.Trim();
            NameAr = nameAr.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates service descriptions
        /// </summary>
        /// <param name="descriptionEn">New description in English</param>
        /// <param name="descriptionAr">New description in Arabic</param>
        public void UpdateDescriptions(string? descriptionEn, string? descriptionAr)
        {
            DescriptionEn = descriptionEn?.Trim();
            DescriptionAr = descriptionAr?.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the service price
        /// </summary>
        /// <param name="newPrice">New price</param>
        public void UpdatePrice(decimal newPrice)
        {
            ValidatePrice(newPrice);

            BasePrice = newPrice;
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates the estimated duration
        /// </summary>
        /// <param name="newDuration">New duration in minutes</param>
        public void UpdateDuration(int newDuration)
        {
            ValidateDuration(newDuration);

            EstimatedDurationMinutes = newDuration;
            MarkAsUpdated();
        }

        /// <summary>
        /// Changes the service type
        /// </summary>
        /// <param name="newType">New service type</param>
        public void ChangeType(ServiceType newType)
        {
            ServiceType = newType;
            MarkAsUpdated();
        }

        // <summary>
        /// Activates the service
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                return; // Already active

            IsActive = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Deactivates the service
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                return; // Already inactive

            IsActive = false;
            MarkAsUpdated();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates service name
        /// </summary>
        private static void ValidateName(string name, string language)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"{language} name cannot be empty");

            if (name.Length < 3)
                throw new ArgumentException($"{language} name must be at least 3 characters");

            if (name.Length > 200)
                throw new ArgumentException($"{language} name cannot exceed 200 characters");
        }

        /// <summary>
        /// Validates service price
        /// </summary>
        private static void ValidatePrice(decimal price)
        {
            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            if (price > 1000000) // Reasonable upper limit
                throw new ArgumentException("Price cannot exceed 1,000,000", nameof(price));
        }

        /// <summary>
        /// Validates service duration
        /// </summary>
        private static void ValidateDuration(int duration)
        {
            if (duration <= 0)
                throw new ArgumentException("Duration must be positive", nameof(duration));

            if (duration > 1440) // Max 24 hours (1 day)
                throw new ArgumentException("Duration cannot exceed 1440 minutes (24 hours)", nameof(duration));
        }

        #endregion




    }
}