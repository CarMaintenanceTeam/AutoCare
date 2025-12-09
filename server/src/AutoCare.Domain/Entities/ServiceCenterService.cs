using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Junction entity linking ServiceCenters to Services (Many-to-Many)
    /// Allows custom pricing per service center
    /// </summary>
    public sealed class ServiceCenterService : AuditableEntity<int>
    {

        #region Properties

        /// <summary>
        /// Gets the service center ID
        /// </summary>
        public int ServiceCenterId { get; private set; }

        /// <summary>
        /// Gets the service ID
        /// </summary>
        public int ServiceId { get; private set; }

        /// <summary>
        /// Gets the custom price for this service at this center (overrides base price if set)
        /// </summary>
        public decimal? CustomPrice { get; private set; }

        /// <summary>
        /// Gets whether this service is currently available at this center
        /// </summary>
        public bool IsAvailable { get; private set; } = true;

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the service center
        /// </summary>
        public ServiceCenter ServiceCenter { get; private set; } = null!;

        /// <summary>
        /// Gets the service
        /// </summary>
        public Service Service { get; private set; } = null!;

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private ServiceCenterService()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new service center service link
        /// </summary>
        /// <param name="serviceCenterId">Service center ID</param>
        /// <param name="serviceId">Service ID</param>
        /// <param name="customPrice">Optional custom price (overrides base price)</param>
        /// <returns>A new ServiceCenterService entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static ServiceCenterService Create(
            int serviceCenterId,
            int serviceId,
            decimal? customPrice = null)
        {
            if (serviceCenterId <= 0)
                throw new ArgumentException("Service center ID must be positive", nameof(serviceCenterId));

            if (serviceId <= 0)
                throw new ArgumentException("Service ID must be positive", nameof(serviceId));

            if (customPrice.HasValue && customPrice.Value < 0)
                throw new ArgumentException("Custom price cannot be negative", nameof(customPrice));

            var serviceCenterService = new ServiceCenterService
            {
                ServiceCenterId = serviceCenterId,
                ServiceId = serviceId,
                CustomPrice = customPrice,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            return serviceCenterService;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Updates the custom price
        /// </summary>
        /// <param name="newPrice">New custom price (null to use base price)</param>
        public void UpdateCustomPrice(decimal? newPrice)
        {
            if (newPrice.HasValue && newPrice.Value < 0)
                throw new ArgumentException("Custom price cannot be negative", nameof(newPrice));

            CustomPrice = newPrice;
            MarkAsUpdated();
        }

        /// <summary>
        /// Makes the service available at this center
        /// </summary>
        public void MakeAvailable()
        {
            if (IsAvailable)
                return; // Already available

            IsAvailable = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Makes the service unavailable at this center
        /// </summary>
        public void MakeUnavailable()
        {
            if (!IsAvailable)
                return; // Already unavailable

            IsAvailable = false;
            MarkAsUpdated();
        }

        /// <summary>
        /// Gets the effective price (custom price if set, otherwise base price)
        /// </summary>
        /// <returns>The effective price</returns>
        public decimal GetEffectivePrice()
        {
            return CustomPrice ?? Service.BasePrice;
        }

        #endregion

    }
}




