using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Bookings.Models;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Bookings.Commands.CreateBooking
{
    /// <summary>
    /// Handler for CreateBookingCommand
    /// Implements Command Handler pattern with comprehensive business rule validation
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles booking creation
    /// - Business Logic Protection: Enforces all booking rules
    /// - Transaction Safety: All validations before database changes
    /// </summary>
    public sealed class CreateBookingCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService,
        IEmailService emailService,
        ILogger<CreateBookingCommandHandler> logger) : ICommandHandler<CreateBookingCommand, BookingDto>
    {
        private readonly IApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ICurrentUserService _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        private readonly ILogger<CreateBookingCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Handles the create booking command
        /// </summary>
        public async Task<BookingDto> Handle(
            CreateBookingCommand request,
            CancellationToken cancellationToken)
        {
            // Get current user
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to create a booking");

            _logger.LogInformation(
                "Creating booking for UserId: {UserId}, Vehicle: {VehicleId}, Service: {ServiceId}",
                userId,
                request.VehicleId,
                request.ServiceId);

            // Get customer
            var customer = await GetCustomer(userId, cancellationToken);

            // Validate vehicle ownership
            var vehicle = await ValidateVehicleOwnership(request.VehicleId, customer.Id, cancellationToken);

            // Validate service center
            var serviceCenter = await ValidateServiceCenter(request.ServiceCenterId, cancellationToken);

            // Validate service availability
            var serviceCenterService = await ValidateServiceAvailability(
                request.ServiceCenterId,
                request.ServiceId,
                cancellationToken);

            // Check for double booking
            await ValidateNoDoubleBooking(
                request.ServiceCenterId,
                request.BookingDate,
                request.BookingTime,
                cancellationToken);

            // Create booking entity
            var booking = Booking.Create(
                customerId: customer.Id,
                vehicleId: request.VehicleId,
                serviceCenterId: request.ServiceCenterId,
                serviceId: request.ServiceId,
                bookingDate: request.BookingDate,
                bookingTime: request.BookingTime,
                customerNotes: request.CustomerNotes);

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Booking created successfully. BookingId: {BookingId}, BookingNumber: {BookingNumber}",
                booking.Id,
                booking.BookingNumber);

            // Send confirmation email (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendBookingConfirmationAsync(booking.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send booking confirmation email");
                }
            }, cancellationToken);

            // Map to DTO
            return await MapToDto(booking, vehicle, serviceCenter, serviceCenterService);
        }

        /// <summary>
        /// Gets customer for current user
        /// </summary>
        private async Task<Customer> GetCustomer(int userId, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException("Customer profile not found");
            }

            if (!customer.User.IsActive)
            {
                throw new BusinessRuleValidationException("Your account is inactive");
            }

            return customer;
        }

        /// <summary>
        /// Validates vehicle ownership
        /// </summary>
        private async Task<Vehicle> ValidateVehicleOwnership(
            int vehicleId,
            int customerId,
            CancellationToken cancellationToken)
        {
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId, cancellationToken);

            if (vehicle == null)
            {
                throw new NotFoundException("Vehicle", vehicleId);
            }

            if (vehicle.CustomerId != customerId)
            {
                throw new ForbiddenException("You can only book services for your own vehicles");
            }

            return vehicle;
        }

        /// <summary>
        /// Validates service center
        /// </summary>
        private async Task<ServiceCenter> ValidateServiceCenter(
            int serviceCenterId,
            CancellationToken cancellationToken)
        {
            var serviceCenter = await _context.ServiceCenters
                .FirstOrDefaultAsync(sc => sc.Id == serviceCenterId, cancellationToken);

            if (serviceCenter == null)
            {
                throw new NotFoundException("ServiceCenter", serviceCenterId);
            }

            if (!serviceCenter.IsActive)
            {
                throw new BusinessRuleValidationException(
                    "This service center is currently not accepting bookings");
            }

            return serviceCenter;
        }

        /// <summary>
        /// Validates service availability at service center
        /// </summary>
        private async Task<ServiceCenterService> ValidateServiceAvailability(
            int serviceCenterId,
            int serviceId,
            CancellationToken cancellationToken)
        {
            var serviceCenterService = await _context.ServiceCenterServices
                .Include(scs => scs.Service)
                .FirstOrDefaultAsync(scs =>
                    scs.ServiceCenterId == serviceCenterId &&
                    scs.ServiceId == serviceId,
                    cancellationToken);

            if (serviceCenterService == null)
            {
                throw new BusinessRuleValidationException(
                    "This service is not offered at the selected service center");
            }

            if (!serviceCenterService.IsAvailable)
            {
                throw new BusinessRuleValidationException(
                    "This service is currently unavailable at the selected service center");
            }

            if (!serviceCenterService.Service.IsActive)
            {
                throw new BusinessRuleValidationException(
                    "This service is currently not available");
            }

            return serviceCenterService;
        }

        /// <summary>
        /// Validates no double booking exists
        /// </summary>
        private async Task ValidateNoDoubleBooking(
            int serviceCenterId,
            DateTime bookingDate,
            TimeSpan bookingTime,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Bookings
                .AnyAsync(b =>
                    b.ServiceCenterId == serviceCenterId &&
                    b.BookingDate == bookingDate.Date &&
                    b.BookingTime == bookingTime &&
                    (b.Status == Domain.Enums.BookingStatus.Pending ||
                     b.Status == Domain.Enums.BookingStatus.Confirmed ||
                     b.Status == Domain.Enums.BookingStatus.InProgress),
                    cancellationToken);

            if (exists)
            {
                throw new BusinessRuleValidationException(
                    "This time slot is already booked. Please select a different time.");
            }
        }

        /// <summary>
        /// Maps booking to DTO
        /// </summary>
        private async Task<BookingDto> MapToDto(
            Booking booking,
            Vehicle vehicle,
            ServiceCenter serviceCenter,
            ServiceCenterService serviceCenterService)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstAsync(c => c.Id == booking.CustomerId);

            var effectivePrice = serviceCenterService.CustomPrice ?? serviceCenterService.Service.BasePrice;


            return new BookingDto
            {
                Id = booking.Id,
                BookingNumber = booking.BookingNumber,
                CustomerId = booking.CustomerId,
                CustomerName = customer.User.FullName,
                CustomerEmail = customer.User.Email,
                CustomerPhone = customer.User.PhoneNumber,
                VehicleId = vehicle.Id,
                VehicleInfo = $"{vehicle.Brand} {vehicle.Model} ({vehicle.Year})",
                PlateNumber = vehicle.PlateNumber,
                ServiceCenterId = serviceCenter.Id,
                ServiceCenterName = serviceCenter.NameEn,
                ServiceCenterAddress = serviceCenter.AddressEn,
                ServiceCenterPhone = serviceCenter.PhoneNumber,
                ServiceId = serviceCenterService.ServiceId,
                ServiceName = serviceCenterService.Service.NameEn,
                ServicePrice = effectivePrice,
                EstimatedDurationMinutes = serviceCenterService.Service.EstimatedDurationMinutes,
                BookingDate = booking.BookingDate,
                BookingTime = booking.BookingTime,
                Status = booking.Status.ToString(),
                CustomerNotes = booking.CustomerNotes,
                StaffNotes = booking.StaffNotes,
                ConfirmedAt = booking.ConfirmedAt,
                ConfirmedBy = booking.ConfirmedBy,
                CompletedAt = booking.CompletedAt,
                CancelledAt = booking.CancelledAt,
                CancellationReason = booking.CancellationReason,
                CanBeModified = booking.CanBeModified(),
                CanBeCancelledByCustomer = booking.CanBeCancelledByCustomer(),
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }
    }
}