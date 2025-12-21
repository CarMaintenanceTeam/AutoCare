using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.Bookings.Models
{
    /// <summary>
    /// Simplified DTO for booking list view
    /// Contains essential information for listing
    /// </summary>
    public sealed class BookingListDto
    {
        /// <summary>
        /// Gets or sets the booking ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the booking number
        /// </summary>
        public string BookingNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the vehicle info
        /// </summary>
        public string VehicleInfo { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service center name
        /// </summary>
        public string ServiceCenterName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the service name
        /// </summary>
        public string ServiceName { get; set; } = string.Empty;

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
        /// Gets or sets the service price
        /// </summary>
        public decimal ServicePrice { get; set; }

        /// <summary>
        /// Gets or sets when created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the combined booking date and time
        /// </summary>
        public DateTime BookingDateTime => BookingDate.Add(BookingTime);
    }
}