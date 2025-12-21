using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.ServiceCenters.Models;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Services.Queries.GetServicesByServiceCenter
{
    /// <summary>
    /// Handler for GetServicesByServiceCenterQuery
    /// Retrieves all services offered at a specific service center
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves services for one center
    /// - Fail Fast: Throws NotFoundException if center not found
    /// - Performance: Uses projection for efficient queries
    /// </summary>
    public sealed class GetServicesByServiceCenterQueryHandler
        : IQueryHandler<GetServicesByServiceCenterQuery, List<ServiceCenterServiceDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetServicesByServiceCenterQueryHandler> _logger;

        public GetServicesByServiceCenterQueryHandler(
            IApplicationDbContext context,
            ILogger<GetServicesByServiceCenterQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the query to get services by service center
        /// </summary>
        /// <param name="request">Query with service center ID and filters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>List of services with pricing</returns>
        /// <exception cref="NotFoundException">Thrown when service center not found</exception>
        public async Task<List<ServiceCenterServiceDto>> Handle(
            GetServicesByServiceCenterQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving services for service center ID: {ServiceCenterId}",
                request.ServiceCenterId);

            // Check if service center exists
            var serviceCenterExists = await _context.ServiceCenters
                .AnyAsync(sc => sc.Id == request.ServiceCenterId, cancellationToken);

            if (!serviceCenterExists)
            {
                _logger.LogWarning(
                    "Service center not found with ID: {ServiceCenterId}",
                    request.ServiceCenterId);

                throw new NotFoundException("ServiceCenter", request.ServiceCenterId);
            }

            // Build query
            var query = _context.ServiceCenterServices
                .Include(scs => scs.Service)
                .Where(scs => scs.ServiceCenterId == request.ServiceCenterId)
                .Where(scs => scs.Service.IsActive); // Only active services

            // Filter by availability
            if (!request.IncludeUnavailable)
            {
                query = query.Where(scs => scs.IsAvailable);
            }

            // Filter by service type
            if (!string.IsNullOrWhiteSpace(request.ServiceType))
            {
                if (Enum.TryParse<ServiceType>(request.ServiceType, true, out var serviceType))
                {
                    query = query.Where(scs => scs.Service.ServiceType == serviceType);
                }
            }

            // Project to DTO
            var services = await query
                .Select(scs => new ServiceCenterServiceDto
                {
                    ServiceId = scs.ServiceId,
                    NameEn = scs.Service.NameEn,
                    NameAr = scs.Service.NameAr,
                    DescriptionEn = scs.Service.DescriptionEn,
                    DescriptionAr = scs.Service.DescriptionAr,
                    Price = scs.CustomPrice ?? scs.Service.BasePrice,
                    BasePrice = scs.Service.BasePrice,
                    EstimatedDurationMinutes = scs.Service.EstimatedDurationMinutes,
                    ServiceType = scs.Service.ServiceType.ToString(),
                    IsAvailable = scs.IsAvailable
                })
                .OrderBy(s => s.NameEn)
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} services for service center ID: {ServiceCenterId}",
                services.Count,
                request.ServiceCenterId);

            return services;
        }
    }
}