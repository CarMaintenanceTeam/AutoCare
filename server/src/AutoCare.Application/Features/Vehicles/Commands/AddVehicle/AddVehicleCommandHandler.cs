using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Vehicles.Models;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Vehicles.Commands.AddVehicle
{
    /// <summary>
    /// Handler for AddVehicleCommand
    /// Implements Command Handler pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles vehicle creation
    /// - Dependency Inversion: Depends on abstractions
    /// - Fail Fast: Validates before creating entity
    /// 
    /// Security:
    /// - Only allows adding vehicles for authenticated customer
    /// - Validates customer ownership
    /// - Enforces uniqueness constraints
    /// </summary>
    public sealed class AddVehicleCommandHandler : ICommandHandler<AddVehicleCommand, VehicleDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AddVehicleCommandHandler> _logger;

        public AddVehicleCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<AddVehicleCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the add vehicle command
        /// </summary>
        /// <param name="request">Add vehicle command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created vehicle DTO</returns>
        /// <exception cref="UnauthorizedException">Thrown when user not authenticated</exception>
        /// <exception cref="NotFoundException">Thrown when customer not found</exception>
        /// <exception cref="DuplicateException">Thrown when plate/VIN already exists</exception>
        public async Task<VehicleDto> Handle(
            AddVehicleCommand request,
            CancellationToken cancellationToken)
        {
            // Get current user ID
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to add a vehicle");

            _logger.LogInformation(
                "Adding vehicle for UserId: {UserId}. Brand: {Brand}, Model: {Model}",
                userId,
                request.Brand,
                request.Model);

            // Get customer
            var customer = await GetCustomer(userId, cancellationToken);

            // Validate uniqueness
            await ValidatePlateNumberUniqueness(request.PlateNumber, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.VIN))
            {
                await ValidateVINUniqueness(request.VIN, cancellationToken);
            }

            // Create vehicle entity
            var vehicle = Vehicle.Create(
                customerId: customer.Id,
                brand: request.Brand,
                model: request.Model,
                year: request.Year,
                plateNumber: request.PlateNumber,
                vin: request.VIN,
                color: request.Color);

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Vehicle added successfully. VehicleId: {VehicleId}, PlateNumber: {PlateNumber}",
                vehicle.Id,
                vehicle.PlateNumber);

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

        /// <summary>
        /// Gets customer for current user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Customer entity</returns>
        /// <exception cref="NotFoundException">Thrown when customer not found</exception>
        private async Task<Customer> GetCustomer(int userId, CancellationToken cancellationToken)
        {
            var customer = await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning(
                    "Customer not found for UserId: {UserId}",
                    userId);

                throw new NotFoundException(
                    "Customer profile not found. Only customers can add vehicles.");
            }

            // Validate user is active
            if (!customer.User.IsActive)
            {
                throw new BusinessRuleValidationException(
                    "Your account is inactive. Please contact support.");
            }

            return customer;
        }

        /// <summary>
        /// Validates that plate number is unique
        /// </summary>
        /// <param name="plateNumber">Plate number to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="DuplicateException">Thrown when plate number exists</exception>
        private async Task ValidatePlateNumberUniqueness(
            string plateNumber,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Vehicles
                .AnyAsync(v => v.PlateNumber.ToLower() == plateNumber.ToLower(),
                         cancellationToken);

            if (exists)
            {
                _logger.LogWarning(
                    "Plate number already exists: {PlateNumber}",
                    plateNumber);

                throw new DuplicateException("Vehicle", "PlateNumber", plateNumber);
            }
        }

        /// <summary>
        /// Validates that VIN is unique
        /// </summary>
        /// <param name="vin">VIN to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="DuplicateException">Thrown when VIN exists</exception>
        private async Task ValidateVINUniqueness(
            string vin,
            CancellationToken cancellationToken)
        {
            var exists = await _context.Vehicles
                .AnyAsync(v => v.VIN != null && v.VIN.ToLower() == vin.ToLower(),
                         cancellationToken);

            if (exists)
            {
                _logger.LogWarning(
                    "VIN already exists: {VIN}",
                    vin);

                throw new DuplicateException("Vehicle", "VIN", vin);
            }
        }
    }
}