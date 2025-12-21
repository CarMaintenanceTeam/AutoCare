using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;

namespace AutoCare.Application.Features.Bookings.Commands.CancelBooking
{
    /// <summary>
    /// Command to cancel a booking
    /// Can be executed by customer or staff
    /// 
    /// Business Rules:
    /// - Customer can cancel Pending or Confirmed bookings
    /// - Staff can cancel any booking except Completed
    /// - Reason is required
    /// - Cannot cancel already cancelled bookings
    /// - Cannot cancel completed bookings
    /// </summary>
    /// <param name="BookingId">Booking ID to cancel</param>
    /// <param name="Reason">Cancellation reason (required)</param>
    public sealed record CancelBookingCommand(
        int BookingId,
        string Reason
    ) : ICommand;
}