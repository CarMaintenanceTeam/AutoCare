using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;

namespace AutoCare.Application.Features.Bookings.Models
{
    /// <summary>
    /// Data Transfer Object for Booking
    /// Contains complete booking information with related entities
    /// </summary>
    public sealed class BookingDto : AuditableEntityDto<int>
    {
        /// <summary>
        /// Gets or sets the unique booking number
        /// </summary>
        public string BookingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer ID
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the customer name
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer email
        /// </summary>
        public string CustomerEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer phone
        /// </summary>
        public string? CustomerPhone { get; set; }

        /// <summary>
        /// Gets or sets the vehicle ID
        /// </summary>
        public int VehicleId { get; set; }

        /// <summary>
        /// Gets or sets the vehicle display name
        /// </summary>
        public string VehicleInfo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle plate number
        /// </summary>
        public string PlateNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service center ID
        /// </summary>
        public int ServiceCenterId { get; set; }

        /// <summary>
        /// Gets or sets the service center name
        /// </summary>
        public string ServiceCenterName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service center address
        /// </summary>
        public string ServiceCenterAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service center phone
        /// </summary>
        public string ServiceCenterPhone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service ID
        /// </summary>
        public int ServiceId { get; set; }

        /// <summary>
        /// Gets or sets the service name
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service price
        /// </summary>
        public decimal ServicePrice { get; set; }

        /// <summary>
        /// Gets or sets the estimated duration
        /// </summary>
        public int EstimatedDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the booking date
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Gets or sets the booking time
        /// </summary>
        public TimeSpan BookingTime { get; set; }

        /// <summary>
        /// Gets or sets the booking status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer notes
        /// </summary>
        public string? CustomerNotes { get; set; }

        /// <summary>
        /// Gets or sets the staff notes
        /// </summary>
        public string? StaffNotes { get; set; }

        /// <summary>
        /// Gets or sets when the booking was confirmed
        /// </summary>
        public DateTime? ConfirmedAt { get; set; }

        /// <summary>
        /// Gets or sets who confirmed the booking
        /// </summary>
        public int? ConfirmedBy { get; set; }

        /// <summary>
        /// Gets or sets when the service was completed
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Gets or sets when the booking was cancelled
        /// </summary>
        public DateTime? CancelledAt { get; set; }

        /// <summary>
        /// Gets or sets the cancellation reason
        /// </summary>
        public string? CancellationReason { get; set; }

        /// <summary>
        /// Gets or sets whether the booking can be modified
        /// </summary>
        public bool CanBeModified { get; set; }

        /// <summary>
        /// Gets or sets whether the booking can be cancelled by customer
        /// </summary>
        public bool CanBeCancelledByCustomer { get; set; }

        /// <summary>
        /// Gets or sets the combined booking date and time
        /// </summary>
        public DateTime BookingDateTime => BookingDate.Add(BookingTime);
    }
}