using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Bookings.Models;

namespace AutoCare.Application.Features.Bookings.Queries.GetBookingById
{
    /// <summary>
    /// Query to get booking details by ID
    /// Authorization: Only owner or staff can view
    /// </summary>
    /// <param name="BookingId">Booking ID</param>
    public sealed record GetBookingByIdQuery(
        int BookingId
    ) : IQuery<BookingDto>;
}