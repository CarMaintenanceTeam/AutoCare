using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Bookings.Models;

namespace AutoCare.Application.Features.Bookings.Commands.CreateBooking
{
    /// <summary>
    /// Command to create a new booking
    /// Implements CQRS Command pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles booking creation
    /// - Immutability: Record type
    /// - Command Pattern: Encapsulates booking request
    /// 
    /// Business Rules:
    /// - Customer must own the vehicle
    /// - Service center must offer the service
    /// - Date must be today or future
    /// - Time must be on 30-minute intervals
    /// - Cannot double-book (same center, date, time)
    /// - Service center must be active
    /// - Service must be active and available
    /// 
    /// Flow:
    /// 1. Validate input (CreateBookingCommandValidator)
    /// 2. Verify customer owns vehicle
    /// 3. Verify service center offers service
    /// 4. Check for double booking
    /// 5. Create booking entity
    /// 6. Save to database
    /// 7. Send confirmation notification
    /// 8. Return booking DTO
    /// </summary>
    /// <param name="VehicleId">Vehicle ID (must be owned by customer)</param>
    /// <param name="ServiceCenterId">Service center ID</param>
    /// <param name="ServiceId">Service ID</param>
    /// <param name="BookingDate">Booking date (today or future)</param>
    /// <param name="BookingTime">Booking time (30-minute intervals)</param>
    /// <param name="CustomerNotes">Optional customer notes</param>
    public sealed record CreateBookingCommand(
        int VehicleId,
        int ServiceCenterId,
        int ServiceId,
        DateTime BookingDate,
        TimeSpan BookingTime,
        string? CustomerNotes
    ) : ICommand<BookingDto>;
}