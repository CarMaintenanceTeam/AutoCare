using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Customer entity - extends User with customer-specific data
    /// One-to-One relationship with User
    /// </summary>
    public sealed class Customer : AuditableEntity<int>
    {
        #region Properties

        /// <summary>
        /// Gets the associated user ID (foreign key)
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Gets the customer's address (optional)
        /// </summary>
        public string? Address { get; private set; }

        /// <summary>
        /// Gets the customer's city (optional)
        /// </summary>
        public string? City { get; private set; }

        /// <summary>
        /// Gets whether the customer is subscribed to newsletters
        /// </summary>
        public bool NewsletterSubscribed { get; private set; }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the associated user account
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// Gets the customer's vehicles
        /// </summary>
        public ICollection<Vehicle> Vehicles { get; private set; } = new List<Vehicle>();

        /// <summary>
        /// Gets the customer's bookings
        /// </summary>
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private Customer()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new customer profile
        /// </summary>
        /// <param name="userId">Associated user ID</param>
        /// <param name="address">Optional address</param>
        /// <param name="city">Optional city</param>
        /// <returns>A new Customer entity</returns>
        public static Customer Create(int userId, string? address = null, string? city = null)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));

            var customer = new Customer
            {
                UserId = userId,
                Address = address?.Trim(),
                City = city?.Trim(),
                NewsletterSubscribed = false,
                CreatedAt = DateTime.UtcNow
            };

            return customer;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Updates the customer's profile information
        /// </summary>
        /// <param name="address">New address</param>
        /// <param name="city">New city</param>
        public void UpdateProfile(string? address, string? city)
        {
            Address = address?.Trim();
            City = city?.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Subscribes the customer to newsletters
        /// </summary>
        public void SubscribeToNewsletter()
        {
            if (NewsletterSubscribed)
                return; // Already subscribed

            NewsletterSubscribed = true;
            MarkAsUpdated();
        }

        /// <summary>
        /// Unsubscribes the customer from newsletters
        /// </summary>
        public void UnsubscribeFromNewsletter()
        {
            if (!NewsletterSubscribed)
                return; // Already unsubscribed

            NewsletterSubscribed = false;
            MarkAsUpdated();
        }

        #endregion

    }
}