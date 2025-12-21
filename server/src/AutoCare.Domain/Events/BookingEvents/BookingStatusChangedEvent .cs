using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.Enums;

namespace AutoCare.Domain.Events.BookingEvents
{
    /// <summary>
    /// Domain event raised when a booking status changes
    /// Triggers: Status history logging, Dashboard updates
    /// </summary>
    public sealed record BookingStatusChangedEvent : IDomainEvent
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
        /// Gets the old status
        /// </summary>
        public BookingStatus OldStatus { get; }

        /// <summary>
        /// Gets the new status
        /// </summary>
        public BookingStatus NewStatus { get; }

        /// <summary>
        /// Gets the ID of the user who changed the status
        /// </summary>
        public int ChangedBy { get; }

        /// <summary>
        /// Gets optional notes about the status change
        /// </summary>
        public string? Notes { get; }

        /// <summary>
        /// Creates a new BookingStatusChangedEvent
        /// </summary>
        public BookingStatusChangedEvent(
            int bookingId,
            BookingStatus oldStatus,
            BookingStatus newStatus,
            int changedBy,
            string? notes = null)
        {
            BookingId = bookingId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedBy = changedBy;
            Notes = notes;
        }
    }
}