using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.Enums;
using AutoCare.Domain.Events.BookingEvents;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Booking entity - Core business entity for appointment management
    /// Represents a customer's appointment for vehicle service
    /// </summary>
    public sealed class Booking : AuditableEntity<int>
    {

        #region Properties

        /// <summary>
        /// Gets the unique booking number for customer reference (e.g., BK20240315143045ABC123)
        /// </summary>
        public string BookingNumber { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the customer ID who made the booking
        /// </summary>
        public int CustomerId { get; private set; }

        /// <summary>
        /// Gets the vehicle ID for this booking
        /// </summary>
        public int VehicleId { get; private set; }

        /// <summary>
        /// Gets the service center ID where service will be performed
        /// </summary>
        public int ServiceCenterId { get; private set; }

        /// <summary>
        /// Gets the service ID to be performed
        /// </summary>
        public int ServiceId { get; private set; }

        /// <summary>
        /// Gets the booking date
        /// </summary>
        public DateTime BookingDate { get; private set; }

        /// <summary>
        /// Gets the booking time
        /// </summary>
        public TimeSpan BookingTime { get; private set; }

        /// <summary>
        /// Gets the current booking status
        /// </summary>
        public BookingStatus Status { get; private set; }

        /// <summary>
        /// Gets the customer's notes or special requests (optional)
        /// </summary>
        public string? CustomerNotes { get; private set; }

        /// <summary>
        /// Gets the staff's internal notes (optional)
        /// </summary>
        public string? StaffNotes { get; private set; }

        /// <summary>
        /// Gets when the booking was confirmed (null if not confirmed yet)
        /// </summary>
        public DateTime? ConfirmedAt { get; private set; }

        /// <summary>
        /// Gets the ID of the employee who confirmed the booking
        /// </summary>
        public int? ConfirmedBy { get; private set; }

        /// <summary>
        /// Gets when the service was completed (null if not completed yet)
        /// </summary>
        public DateTime? CompletedAt { get; private set; }

        /// <summary>
        /// Gets when the booking was cancelled (null if not cancelled)
        /// </summary>
        public DateTime? CancelledAt { get; private set; }

        /// <summary>
        /// Gets the reason for cancellation (required if cancelled)
        /// </summary>
        public string? CancellationReason { get; private set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the customer who made the booking
        /// </summary>
        public Customer Customer { get; private set; } = null!;

        /// <summary>
        /// Gets the vehicle for this booking
        /// </summary>
        public Vehicle Vehicle { get; private set; } = null!;

        /// <summary>
        /// Gets the service center
        /// </summary>
        public ServiceCenter ServiceCenter { get; private set; } = null!;

        /// <summary>
        /// Gets the service
        /// </summary>
        public Service Service { get; private set; } = null!;

        /// <summary>
        /// Gets the employee who confirmed the booking (if confirmed)
        /// </summary>
        public User? ConfirmedByUser { get; private set; }


        /// <summary>
        /// Gets the service center service configuration (for custom pricing)
        /// NOTE: This is not mapped to database - use manual join when needed
        /// </summary>
        // public ServiceCenterService? ServiceCenterService { get; private set; }

        /// <summary>
        /// Gets the status change history for this booking
        /// </summary>
        public ICollection<BookingStatusHistory> StatusHistory { get; private set; } = new List<BookingStatusHistory>();

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private Booking()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new booking
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <param name="vehicleId">Vehicle ID</param>
        /// <param name="serviceCenterId">Service center ID</param>
        /// <param name="serviceId">Service ID</param>
        /// <param name="bookingDate">Booking date</param>
        /// <param name="bookingTime">Booking time</param>
        /// <param name="customerNotes">Optional customer notes</param>
        /// <returns>A new Booking entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static Booking Create(
            int customerId,
            int vehicleId,
            int serviceCenterId,
            int serviceId,
            DateTime bookingDate,
            TimeSpan bookingTime,
            string? customerNotes = null)
        {
            // Validate IDs
            if (customerId <= 0)
                throw new ArgumentException("Customer ID must be positive", nameof(customerId));

            if (vehicleId <= 0)
                throw new ArgumentException("Vehicle ID must be positive", nameof(vehicleId));

            if (serviceCenterId <= 0)
                throw new ArgumentException("Service center ID must be positive", nameof(serviceCenterId));

            if (serviceId <= 0)
                throw new ArgumentException("Service ID must be positive", nameof(serviceId));

            // Validate booking date and time
            ValidateBookingDate(bookingDate);
            ValidateBookingTime(bookingTime);

            var booking = new Booking
            {
                BookingNumber = GenerateBookingNumber(),
                CustomerId = customerId,
                VehicleId = vehicleId,
                ServiceCenterId = serviceCenterId,
                ServiceId = serviceId,
                BookingDate = bookingDate.Date, // Store only date part
                BookingTime = bookingTime,
                Status = BookingStatus.Pending,
                CustomerNotes = customerNotes?.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            // Raise domain event
            booking.AddDomainEvent(new BookingCreatedEvent(
                booking.Id,
                booking.BookingNumber,
                booking.CustomerId,
                booking.ServiceCenterId,
                booking.BookingDate,
                booking.BookingTime));

            return booking;
        }

        #endregion

        #region Business Methods - Status Transitions

        /// <summary>
        /// Confirms the booking (Pending → Confirmed)
        /// Can only be done by service center staff
        /// </summary>
        /// <param name="confirmedBy">Employee ID who confirmed</param>
        /// <param name="staffNotes">Optional staff notes</param>
        /// <exception cref="InvalidOperationException">Thrown when status transition is invalid</exception>
        public void Confirm(int confirmedBy, string? staffNotes = null)
        {
            if (Status != BookingStatus.Pending)
                throw new InvalidOperationException(
                    $"Cannot confirm booking in {Status} status. Only Pending bookings can be confirmed.");

            if (confirmedBy <= 0)
                throw new ArgumentException("Confirmed by employee ID must be positive", nameof(confirmedBy));

            var oldStatus = Status;

            Status = BookingStatus.Confirmed;
            ConfirmedBy = confirmedBy;
            ConfirmedAt = DateTime.UtcNow;
            StaffNotes = staffNotes?.Trim();

            MarkAsUpdated();

            // Add to status history
            AddStatusHistoryEntry(oldStatus, confirmedBy, "Booking confirmed by staff");

            // Raise domain event
            AddDomainEvent(new BookingConfirmedEvent(
                Id,
                BookingNumber,
                confirmedBy,
                ConfirmedAt.Value));
        }

        /// <summary>
        /// Starts the service work (Confirmed → InProgress)
        /// </summary>
        /// <param name="userId">Employee ID who started the work</param>
        /// <exception cref="InvalidOperationException">Thrown when status transition is invalid</exception>
        public void StartProgress(int userId)
        {
            if (Status != BookingStatus.Confirmed)
                throw new InvalidOperationException(
                    $"Cannot start work on booking in {Status} status. Only Confirmed bookings can be started.");

            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));

            var oldStatus = Status;

            Status = BookingStatus.InProgress;
            MarkAsUpdated();

            // Add to status history
            AddStatusHistoryEntry(oldStatus, userId, "Service work started");

            // Raise domain event
            AddDomainEvent(new BookingStatusChangedEvent(
                Id,
                oldStatus,
                Status,
                userId,
                "Work started"));
        }

        /// <summary>
        /// Completes the booking (InProgress → Completed)
        /// Terminal state - no further changes allowed
        /// </summary>
        /// <param name="userId">Employee ID who completed the work</param>
        /// <exception cref="InvalidOperationException">Thrown when status transition is invalid</exception>
        public void Complete(int userId)
        {
            if (Status != BookingStatus.InProgress)
                throw new InvalidOperationException(
                    $"Cannot complete booking in {Status} status. Only InProgress bookings can be completed.");

            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));

            var oldStatus = Status;

            Status = BookingStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            MarkAsUpdated();

            // Add to status history
            AddStatusHistoryEntry(oldStatus, userId, "Service completed successfully");

            // Raise domain event
            AddDomainEvent(new BookingCompletedEvent(
                Id,
                BookingNumber,
                CompletedAt.Value));
        }

        /// <summary>
        /// Cancels the booking (Any status except Completed → Cancelled)
        /// Terminal state - no further changes allowed
        /// </summary>
        /// <param name="reason">Reason for cancellation (required)</param>
        /// <param name="cancelledBy">User ID who cancelled</param>
        /// <exception cref="InvalidOperationException">Thrown when cancellation is not allowed</exception>
        public void Cancel(string reason, int cancelledBy)
        {
            // Cannot cancel completed bookings
            if (Status == BookingStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed booking.");

            // Cannot cancel already cancelled bookings
            if (Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Booking is already cancelled.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            if (cancelledBy <= 0)
                throw new ArgumentException("Cancelled by user ID must be positive", nameof(cancelledBy));

            var oldStatus = Status;

            Status = BookingStatus.Cancelled;
            CancellationReason = reason.Trim();
            CancelledAt = DateTime.UtcNow;
            MarkAsUpdated();

            // Add to status history
            AddStatusHistoryEntry(oldStatus, cancelledBy, $"Cancelled: {reason}");

            // Raise domain event
            AddDomainEvent(new BookingCancelledEvent(
                Id,
                BookingNumber,
                CancellationReason,
                cancelledBy,
                CancelledAt.Value));
        }

        #endregion

        #region Business Methods - Updates

        /// <summary>
        /// Updates customer notes
        /// </summary>
        /// <param name="notes">New customer notes</param>
        public void UpdateCustomerNotes(string? notes)
        {
            if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
                throw new InvalidOperationException("Cannot update notes for completed or cancelled bookings.");

            CustomerNotes = notes?.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Updates staff notes
        /// </summary>
        /// <param name="notes">New staff notes</param>
        public void UpdateStaffNotes(string? notes)
        {
            StaffNotes = notes?.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Reschedules the booking to a new date/time
        /// Can only be done for Pending or Confirmed bookings
        /// </summary>
        /// <param name="newDate">New booking date</param>
        /// <param name="newTime">New booking time</param>
        /// <param name="userId">User ID who rescheduled</param>
        public void Reschedule(DateTime newDate, TimeSpan newTime, int userId)
        {
            if (Status != BookingStatus.Pending && Status != BookingStatus.Confirmed)
                throw new InvalidOperationException(
                    "Can only reschedule Pending or Confirmed bookings.");

            ValidateBookingDate(newDate);
            ValidateBookingTime(newTime);

            var oldDate = BookingDate;
            var oldTime = BookingTime;

            BookingDate = newDate.Date;
            BookingTime = newTime;
            MarkAsUpdated();

            // Add to status history
            AddStatusHistoryEntry(
                Status,
                userId,
                $"Rescheduled from {oldDate:yyyy-MM-dd} {oldTime:hh\\:mm} to {newDate:yyyy-MM-dd} {newTime:hh\\:mm}");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Generates a unique booking number
        /// Format: BK{YYYYMMDDHHMMSS}{6-char-random}
        /// Example: BK20240315143045ABC123
        /// </summary>
        private static string GenerateBookingNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var randomPart = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
            return $"BK{timestamp}{randomPart}";
        }

        /// <summary>
        /// Adds an entry to the status history
        /// </summary>
        private void AddStatusHistoryEntry(BookingStatus oldStatus, int changedBy, string? notes)
        {
            var history = BookingStatusHistory.Create(
                Id,
                oldStatus.ToString(),
                Status.ToString(),
                changedBy,
                notes);

            StatusHistory.Add(history);
        }

        /// <summary>
        /// Checks if the booking can be modified
        /// </summary>
        public bool CanBeModified()
        {
            return Status != BookingStatus.Completed && Status != BookingStatus.Cancelled;
        }

        /// <summary>
        /// Checks if the booking can be cancelled by customer
        /// </summary>
        public bool CanBeCancelledByCustomer()
        {
            // Customer can cancel Pending or Confirmed bookings
            // Cannot cancel InProgress or already completed/cancelled
            return Status == BookingStatus.Pending || Status == BookingStatus.Confirmed;
        }

        /// <summary>
        /// Gets the booking datetime combined
        /// </summary>
        public DateTime GetBookingDateTime()
        {
            return BookingDate.Add(BookingTime);
        }

        /// <summary>
        /// Checks if booking is overdue (past scheduled time and still pending/confirmed)
        /// </summary>
        public bool IsOverdue()
        {
            if (Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
                return false;

            var scheduledDateTime = GetBookingDateTime();
            return DateTime.UtcNow > scheduledDateTime;
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates booking date (must be today or future)
        /// </summary>
        private static void ValidateBookingDate(DateTime date)
        {
            var today = DateTime.UtcNow.Date;

            if (date.Date < today)
                throw new ArgumentException(
                    "Booking date cannot be in the past",
                    nameof(date));

            // Optional: Limit how far in the future bookings can be made (e.g., 3 months)
            var maxFutureDate = today.AddMonths(3);
            if (date.Date > maxFutureDate)
                throw new ArgumentException(
                    $"Booking date cannot be more than 3 months in the future",
                    nameof(date));
        }

        /// <summary>
        /// Validates booking time
        /// </summary>
        private static void ValidateBookingTime(TimeSpan time)
        {
            if (time < TimeSpan.Zero || time >= TimeSpan.FromHours(24))
                throw new ArgumentException(
                    "Booking time must be between 00:00 and 23:59",
                    nameof(time));

            // Optional: Validate time increments (e.g., must be on 30-minute intervals)
            if (time.Minutes % 30 != 0)
                throw new ArgumentException(
                    "Booking time must be on 30-minute intervals (e.g., 09:00, 09:30)",
                    nameof(time));
        }

        #endregion

    }
}








