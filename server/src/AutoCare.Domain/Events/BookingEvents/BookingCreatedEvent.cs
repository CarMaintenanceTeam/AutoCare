using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.BookingEvents
{
    /// <summary>
    /// Domain event raised when a new booking is created
    /// Triggers: Email notification, SMS notification, Staff notification
    /// </summary>
    public sealed record BookingCreatedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        public Guid EventId { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets when this event occurred
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the booking ID
        /// </summary>
        public int BookingId { get; }

        /// <summary>
        /// Gets the booking number for customer reference
        /// </summary>
        public string BookingNumber { get; }

        /// <summary>
        /// Gets the customer ID who created the booking
        /// </summary>
        public int CustomerId { get; }

        /// <summary>
        /// Gets the service center ID where service will be performed
        /// </summary>
        public int ServiceCenterId { get; }

        /// <summary>
        /// Gets the booking date
        /// </summary>
        public DateTime BookingDate { get; }

        /// <summary>
        /// Gets the booking time
        /// </summary>
        public TimeSpan BookingTime { get; }

        /// <summary>
        /// Creates a new BookingCreatedEvent
        /// </summary>
        public BookingCreatedEvent(
            int bookingId,
            string bookingNumber,
            int customerId,
            int serviceCenterId,
            DateTime bookingDate,
            TimeSpan bookingTime)
        {
            BookingId = bookingId;
            BookingNumber = bookingNumber;
            CustomerId = customerId;
            ServiceCenterId = serviceCenterId;
            BookingDate = bookingDate;
            BookingTime = bookingTime;
        }

    }
}