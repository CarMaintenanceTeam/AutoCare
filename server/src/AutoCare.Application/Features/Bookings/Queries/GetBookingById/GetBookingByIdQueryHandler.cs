using System.Threading.Tasks.Dataflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Bookings.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoCare.Domain.Enums;

namespace AutoCare.Application.Features.Bookings.Queries.GetBookingById
{
    /// <summary>
    /// Handler for GetBookingByIdQuery
    /// Implements Query Handler pattern
    /// Design Principles:
    /// - Single Responsibility: Only retrieves booking by ID
    /// - Dependency Inversion: Depends on abstractions
    /// - Security: Validates user permissions
    /// - Performance: Uses efficient querying and projection
    /// Query Optimization:
    /// - Select only needed fields (projection)
    /// - Efficient filtering with indexes
    /// - Deferred execution with IQueryable
    /// - Batch loading with Include
    /// </summary>
    /// <typeparam name="GetBookingByIdQueryHandler"></typeparam>
    public sealed class GetBookingByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        ILogger<GetBookingByIdQueryHandler> logger) : IQueryHandler<GetBookingByIdQuery, BookingDto>
    {
        private readonly IApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ICurrentUserService _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        private readonly ILogger<GetBookingByIdQueryHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<BookingDto> Handle(
            GetBookingByIdQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in");

            _logger.LogInformation(
                "Retrieving booking {BookingId} for UserId: {UserId}",
                request.BookingId,
                userId);

            // Get booking with all related data
            var booking = await (
               from b in _context.Bookings
                   .Include(b => b.Customer).ThenInclude(c => c.User)
                   .Include(b => b.Vehicle)
                   .Include(b => b.ServiceCenter)
                   .Include(b => b.Service)
               join scs in _context.ServiceCenterServices
                   on new { b.ServiceCenterId, b.ServiceId }
                   equals new { scs.ServiceCenterId, scs.ServiceId }
                   into serviceCenterServices
               from scs in serviceCenterServices.DefaultIfEmpty()
               where b.Id == request.BookingId
               select new BookingDto
               {
                   Id = b.Id,
                   BookingNumber = b.BookingNumber,
                   CustomerId = b.CustomerId,
                   CustomerName = b.Customer.User.FullName,
                   CustomerEmail = b.Customer.User.Email,
                   CustomerPhone = b.Customer.User.PhoneNumber,
                   VehicleId = b.VehicleId,
                   VehicleInfo = $"{b.Vehicle.Brand} {b.Vehicle.Model} ({b.Vehicle.Year})",
                   PlateNumber = b.Vehicle.PlateNumber,
                   ServiceCenterId = b.ServiceCenterId,
                   ServiceCenterName = b.ServiceCenter.NameEn,
                   ServiceCenterAddress = b.ServiceCenter.AddressEn,
                   ServiceCenterPhone = b.ServiceCenter.PhoneNumber,
                   ServiceId = b.ServiceId,
                   ServiceName = b.Service.NameEn,
                   ServicePrice = scs != null && scs.CustomPrice.HasValue
                       ? scs.CustomPrice.Value
                       : b.Service.BasePrice,
                   EstimatedDurationMinutes = b.Service.EstimatedDurationMinutes,
                   BookingDate = b.BookingDate,
                   BookingTime = b.BookingTime,
                   Status = b.Status.ToString(),
                   CustomerNotes = b.CustomerNotes,
                   StaffNotes = b.StaffNotes,
                   ConfirmedAt = b.ConfirmedAt,
                   ConfirmedBy = b.ConfirmedBy,
                   CompletedAt = b.CompletedAt,
                   CancelledAt = b.CancelledAt,
                   CancellationReason = b.CancellationReason,
                   CanBeModified = b.CanBeModified(),
                   CanBeCancelledByCustomer = b.CanBeCancelledByCustomer(),
                   CreatedAt = b.CreatedAt,
                   UpdatedAt = b.UpdatedAt
               }
           ).FirstOrDefaultAsync(cancellationToken);

            if (booking == null)
            {
                throw new NotFoundException("Booking", request.BookingId);
            }

            // Check authorization
            var isOwner = await _context.Customers
                .AnyAsync(c => c.Id == booking.CustomerId && c.UserId == userId, cancellationToken);

            // var isEmployee = _currentUserService.UserType == "Employee";
            var userType = _currentUserService.UserType;
            var isStaff = !string.IsNullOrWhiteSpace(userType) &&
                (userType.Equals(UserType.Employee.ToString(), StringComparison.OrdinalIgnoreCase) ||
                 userType.Equals(UserType.Admin.ToString(), StringComparison.OrdinalIgnoreCase));
            if (!isOwner && !isStaff)
            {
                throw new ForbiddenException("You don't have permission to view this booking");
            }

            _logger.LogInformation("Retrieved booking: {BookingNumber}", booking.BookingNumber);

            return booking;
        }
    }
}