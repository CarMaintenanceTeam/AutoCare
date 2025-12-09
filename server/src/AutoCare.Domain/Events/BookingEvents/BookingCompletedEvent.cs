using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.BookingEvents
{
    /// <summary>
    /// Domain event raised when a booking is completed
    /// Triggers: Completion email, Request for feedback/review
    /// </summary>
    public sealed record BookingCompletedEvent : IDomainEvent
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
        /// Gets when the booking was completed
        /// </summary>
        public DateTime CompletedAt { get; }

        /// <summary>
        /// Creates a new BookingCompletedEvent
        /// </summary>
        public BookingCompletedEvent(
            int bookingId,
            string bookingNumber,
            DateTime completedAt)
        {
            BookingId = bookingId;
            BookingNumber = bookingNumber;
            CompletedAt = completedAt;
        }

    }
}