using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Vehicles.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Vehicles.Commands.UpdateVehicle
{
    /// <summary>
    /// Handler for UpdateVehicleCommand
    /// Implements Command Handler pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles vehicle updates
    /// - Authorization: Validates ownership
    /// - Immutability: Doesn't allow changing unique identifiers
    /// </summary>
    public sealed class UpdateVehicleCommandHandler : ICommandHandler<UpdateVehicleCommand, VehicleDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateVehicleCommandHandler> _logger;

        public UpdateVehicleCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<UpdateVehicleCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the update vehicle command
        /// </summary>
        /// <param name="request">Update vehicle command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Updated vehicle DTO</returns>
        /// <exception cref="UnauthorizedException">Thrown when user not authenticated</exception>
        /// <exception cref="NotFoundException">Thrown when vehicle not found</exception>
        /// <exception cref="ForbiddenException">Thrown when user doesn't own vehicle</exception>
        public async Task<VehicleDto> Handle(
            UpdateVehicleCommand request,
            CancellationToken cancellationToken)
        {
            // Get current user ID
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to update a vehicle");

            _logger.LogInformation(
                "Updating vehicle {VehicleId} for UserId: {UserId}",
                request.VehicleId,
                userId);

            // Get customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException("Customer profile not found");
            }

            // Get vehicle with ownership check
            var vehicle = await _context.Vehicles
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
                    "User {UserId} attempted to update vehicle {VehicleId} owned by customer {CustomerId}",
                    userId,
                    request.VehicleId,
                    vehicle.CustomerId);

                throw new ForbiddenException(
                    "You can only update your own vehicles");
            }

            // Update vehicle details
            vehicle.UpdateDetails(
                brand: request.Brand,
                model: request.Model,
                year: request.Year,
                color: request.Color);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle updated successfully. VehicleId: {VehicleId}",
                vehicle.Id);

            // Map to DTO
            return new VehicleDto
            {
                Id = vehicle.Id,
                CustomerId = vehicle.CustomerId,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Year = vehicle.Year,
                PlateNumber = vehicle.PlateNumber,
                VIN = vehicle.VIN,
                Color = vehicle.Color,
                CreatedAt = vehicle.CreatedAt,
                UpdatedAt = vehicle.UpdatedAt
            };
        }
    }
}