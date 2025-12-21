using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Vehicles.Commands.DeleteVehicle
{
    /// <summary>
    /// Handler for DeleteVehicleCommand
    /// Implements Command Handler pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles vehicle deletion
    /// - Business Logic Protection: Prevents deletion with active bookings
    /// - Authorization: Validates ownership
    /// </summary>
    public sealed class DeleteVehicleCommandHandler : ICommandHandler<DeleteVehicleCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<DeleteVehicleCommandHandler> _logger;

        public DeleteVehicleCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<DeleteVehicleCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the delete vehicle command
        /// </summary>
        /// <param name="request">Delete vehicle command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="UnauthorizedException">Thrown when user not authenticated</exception>
        /// <exception cref="NotFoundException">Thrown when vehicle not found</exception>
        /// <exception cref="ForbiddenException">Thrown when user doesn't own vehicle</exception>
        /// <exception cref="BusinessRuleValidationException">Thrown when vehicle has active bookings</exception>
        public async Task Handle(
            DeleteVehicleCommand request,
            CancellationToken cancellationToken)
        {
            // Get current user ID
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to delete a vehicle");

            _logger.LogInformation(
                "Deleting vehicle {VehicleId} for UserId: {UserId}",
                request.VehicleId,
                userId);

            // Get customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException("Customer profile not found");
            }

            // Get vehicle with bookings
            var vehicle = await _context.Vehicles
                .Include(v => v.Bookings)
                .FirstOrDefaultAsync(v => v.Id == request.VehicleId, cancellationToken);

            if (vehicle == null)
            {
                _logger.LogWarning(
                    "Vehicle not found: {VehicleId}",
                    request.VehicleId);

                throw new NotFoundException("Vehicle", request.VehicleId);
            }

            // Verify ownership
            if (vehicle.CustomerId != customer.Id)
            {
                _logger.LogWarning(
                    "User {UserId} attempted to delete vehicle {VehicleId} owned by customer {CustomerId}",
                    userId,
                    request.VehicleId,
                    vehicle.CustomerId);

                throw new ForbiddenException(
                    "You can only delete your own vehicles");
            }

            // Check for active bookings
            ValidateNoActiveBookings(vehicle);

            // Delete vehicle
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle deleted successfully. VehicleId: {VehicleId}, PlateNumber: {PlateNumber}",
                vehicle.Id,
                vehicle.PlateNumber);
        }

        /// <summary>
        /// Validates that vehicle has no active bookings
        /// </summary>
        /// <param name="vehicle">Vehicle to validate</param>
        /// <exception cref="BusinessRuleValidationException">Thrown when active bookings exist</exception>
        private void ValidateNoActiveBookings(Domain.Entities.Vehicle vehicle)
        {
            // Check for active bookings (Pending, Confirmed, InProgress)
            var activeBookings = vehicle.Bookings
                .Where(b => b.Status == BookingStatus.Pending ||
                           b.Status == BookingStatus.Confirmed ||
                           b.Status == BookingStatus.InProgress)
                .ToList();

            if (activeBookings.Count != 0)
            {
                _logger.LogWarning(
                    "Cannot delete vehicle {VehicleId} with {Count} active booking(s)",
                    vehicle.Id,
                    activeBookings.Count);

                throw new BusinessRuleValidationException(
                    $"Cannot delete vehicle with active bookings. " +
                    $"Please cancel or complete {activeBookings.Count} booking(s) first.",
                    new Dictionary<string, object>
                    {
                        ["VehicleId"] = vehicle.Id,
                        ["ActiveBookingsCount"] = activeBookings.Count,
                        ["BookingIds"] = activeBookings.Select(b => b.Id).ToList()
                    });
            }
        }
    }
}