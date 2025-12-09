using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.BookingEvents
{
    /// <summary>
    /// Domain event raised when a booking is cancelled
    /// Triggers: Cancellation email, Refund processing (if applicable)
    /// </summary>
    public sealed record BookingCancelledEvent : IDomainEvent
    {

        /// <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        /// public Guid EventId { get; } = Guid.NewGuid();

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
        /// Gets the reason for cancellation
        /// </summary>
        public string CancellationReason { get; }

        /// <summary>
        /// Gets the ID of the user who cancelled the booking
        /// </summary>
        public int CancelledBy { get; }

        /// <summary>
        /// Gets when the booking was cancelled
        /// </summary>
        public DateTime CancelledAt { get; }

        public Guid EventId => throw new NotImplementedException();

        /// <summary>
        /// Creates a new BookingCancelledEvent
        /// </summary>
        public BookingCancelledEvent(
            int bookingId,
            string bookingNumber,
            string cancellationReason,
            int cancelledBy,
            DateTime cancelledAt)
        {
            BookingId = bookingId;
            BookingNumber = bookingNumber;
            CancellationReason = cancellationReason;
            CancelledBy = cancelledBy;
            CancelledAt = cancelledAt;
        }

    }
}