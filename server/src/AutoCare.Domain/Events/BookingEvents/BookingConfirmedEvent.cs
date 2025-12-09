using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.BookingEvents
{
    /// <summary>
    /// Domain event raised when a booking is confirmed by service center staff
    /// Triggers: Email notification to customer, SMS reminder
    /// </summary>
    public sealed record BookingConfirmedEvent : IDomainEvent
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
        /// Gets the booking number
        /// </summary>
        public string BookingNumber { get; }

        /// <summary>
        /// Gets the ID of the employee who confirmed the booking
        /// </summary>
        public int ConfirmedBy { get; }

        /// <summary>
        /// Gets when the booking was confirmed
        /// </summary>
        public DateTime ConfirmedAt { get; }

        /// <summary>
        /// Creates a new BookingConfirmedEvent
        /// </summary>
        public BookingConfirmedEvent(
            int bookingId,
            string bookingNumber,
            int confirmedBy,
            DateTime confirmedAt)
        {
            BookingId = bookingId;
            BookingNumber = bookingNumber;
            ConfirmedBy = confirmedBy;
            ConfirmedAt = confirmedAt;
        }

    }
}