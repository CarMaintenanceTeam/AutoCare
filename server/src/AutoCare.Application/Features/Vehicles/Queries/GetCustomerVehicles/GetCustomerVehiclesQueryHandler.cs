using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Vehicles.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Vehicles.Queries.GetCustomerVehicles
{
    /// <summary>
    /// Handler for GetCustomerVehiclesQuery
    /// Retrieves all vehicles for authenticated customer
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves customer's vehicles
    /// - Authorization: Automatic filtering by authenticated user
    /// - Performance: Uses projection for efficient queries
    /// </summary>
    public sealed class GetCustomerVehiclesQueryHandler
        : IQueryHandler<GetCustomerVehiclesQuery, List<VehicleListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCustomerVehiclesQueryHandler> _logger;

        public GetCustomerVehiclesQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<GetCustomerVehiclesQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the query to get customer vehicles
        /// </summary>
        /// <param name="request">Query request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of customer's vehicles</returns>
        /// <exception cref="UnauthorizedException">Thrown when user not authenticated</exception>
        /// <exception cref="NotFoundException">Thrown when customer not found</exception>
        public async Task<List<VehicleListDto>> Handle(
            GetCustomerVehiclesQuery request,
            CancellationToken cancellationToken)
        {
            // Get current user ID
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to view your vehicles");

            _logger.LogInformation(
                "Retrieving vehicles for UserId: {UserId}",
                userId);

            // Get customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (customer == null)
            {
                _logger.LogWarning(
                    "Customer not found for UserId: {UserId}",
                    userId);

                throw new NotFoundException("Customer profile not found");
            }

            // Get vehicles with projection
            var vehicles = await _context.Vehicles
                .Where(v => v.CustomerId == customer.Id)
                .OrderByDescending(v => v.CreatedAt)
                .Select(v => new VehicleListDto
                {
                    Id = v.Id,
                    Brand = v.Brand,
                    Model = v.Model,
                    Year = v.Year,
                    PlateNumber = v.PlateNumber,
                    Color = v.Color
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} vehicle(s) for customer {CustomerId}",
                vehicles.Count,
                customer.Id);

            return vehicles;
        }
    }
}