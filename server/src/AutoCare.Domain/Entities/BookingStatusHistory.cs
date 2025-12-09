using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Audit trail entity for booking status changes
    /// Tracks who changed what and when for compliance and debugging
    /// </summary>
    public sealed class BookingStatusHistory : BaseEntity<int>
    {

        #region Properties

        /// <summary>
        /// Gets the booking ID this history entry belongs to
        /// </summary>
        public int BookingId { get; private set; }

        /// <summary>
        /// Gets the previous status (null for initial creation)
        /// </summary>
        public string? OldStatus { get; private set; }

        /// <summary>
        /// Gets the new status after the change
        /// </summary>
        public string NewStatus { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the ID of the user who made the change
        /// </summary>
        public int ChangedBy { get; private set; }

        /// <summary>
        /// Gets when the change occurred
        /// </summary>
        public DateTime ChangedAt { get; private set; }

        /// <summary>
        /// Gets optional notes about the status change
        /// </summary>
        public string? Notes { get; private set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the booking this history entry belongs to
        /// </summary>
        public Booking Booking { get; private set; } = null!;

        /// <summary>
        /// Gets the user who made the change
        /// </summary>
        public User ChangedByUser { get; private set; } = null!;

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private BookingStatusHistory()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new booking status history entry
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <param name="oldStatus">Previous status (null for initial)</param>
        /// <param name="newStatus">New status</param>
        /// <param name="changedBy">User ID who made the change</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>A new BookingStatusHistory entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static BookingStatusHistory Create(
            int bookingId,
            string? oldStatus,
            string newStatus,
            int changedBy,
            string? notes = null)
        {
            if (bookingId <= 0)
                throw new ArgumentException("Booking ID must be positive", nameof(bookingId));

            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("New status cannot be empty", nameof(newStatus));

            if (changedBy <= 0)
                throw new ArgumentException("Changed by user ID must be positive", nameof(changedBy));

            var history = new BookingStatusHistory
            {
                BookingId = bookingId,
                OldStatus = oldStatus?.Trim(),
                NewStatus = newStatus.Trim(),
                ChangedBy = changedBy,
                ChangedAt = DateTime.UtcNow,
                Notes = notes?.Trim()
            };

            return history;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Gets a human-readable description of the status change
        /// </summary>
        /// <returns>Description string</returns>
        public string GetChangeDescription()
        {
            if (string.IsNullOrEmpty(OldStatus))
                return $"Booking created with status '{NewStatus}'";

            return $"Status changed from '{OldStatus}' to '{NewStatus}'";
        }

        /// <summary>
        /// Gets the full audit trail entry as a formatted string
        /// </summary>
        /// <returns>Formatted audit trail entry</returns>
        public string GetAuditTrailEntry()
        {
            var description = GetChangeDescription();
            var notesText = string.IsNullOrEmpty(Notes) ? "" : $" - {Notes}";

            return $"[{ChangedAt:yyyy-MM-dd HH:mm:ss}] {description} by User #{ChangedBy}{notesText}";
        }

        #endregion

    }
}
