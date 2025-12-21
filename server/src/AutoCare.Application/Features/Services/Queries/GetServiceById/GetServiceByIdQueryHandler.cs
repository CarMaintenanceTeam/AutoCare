using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Services.Queries.GetServiceById
{
    /// <summary>
    /// Handler for GetServiceByIdQuery
    /// Retrieves complete service details with optional service centers
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves one service
    /// - Fail Fast: Throws NotFoundException if not found
    /// - Performance: Uses projection and conditional includes
    /// </summary>
    public sealed class GetServiceByIdQueryHandler
        : IQueryHandler<GetServiceByIdQuery, ServiceDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetServiceByIdQueryHandler> _logger;

        public GetServiceByIdQueryHandler(
            IApplicationDbContext context,
            ILogger<GetServiceByIdQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the query to get service by ID
        /// </summary>
        /// <param name="request">Query with service ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Service details</returns>
        /// <exception cref="NotFoundException">Thrown when service not found</exception>
        public async Task<ServiceDto> Handle(
            GetServiceByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving service with ID: {ServiceId}, IncludeServiceCenters: {Include}",
                request.ServiceId,
                request.IncludeServiceCenters);

            // Build query with conditional includes
            var query = _context.Services
                .Where(s => s.Id == request.ServiceId);

            if (request.IncludeServiceCenters)
            {
                query = query.Include(s => s.ServiceCenterServices)
                            .ThenInclude(scs => scs.ServiceCenter);
            }

            // Project to DTO
            var service = await query
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    NameEn = s.NameEn,
                    NameAr = s.NameAr,
                    DescriptionEn = s.DescriptionEn,
                    DescriptionAr = s.DescriptionAr,
                    BasePrice = s.BasePrice,
                    EstimatedDurationMinutes = s.EstimatedDurationMinutes,
                    ServiceType = s.ServiceType.ToString(),
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    ServiceCenters = request.IncludeServiceCenters
                        ? s.ServiceCenterServices
                            .Where(scs => scs.ServiceCenter.IsActive && scs.IsAvailable)
                            .Select(scs => new ServiceCenterForServiceDto
                            {
                                ServiceCenterId = scs.ServiceCenterId,
                                NameEn = scs.ServiceCenter.NameEn,
                                NameAr = scs.ServiceCenter.NameAr,
                                City = scs.ServiceCenter.City,
                                PhoneNumber = scs.ServiceCenter.PhoneNumber,
                                Price = scs.CustomPrice ?? s.BasePrice,
                                IsAvailable = scs.IsAvailable
                            })
                            .OrderBy(sc => sc.NameEn)
                            .ToList()
                        : null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (service == null)
            {
                _logger.LogWarning(
                    "Service not found with ID: {ServiceId}",
                    request.ServiceId);

                throw new NotFoundException("Service", request.ServiceId);
            }

            _logger.LogInformation(
                "Retrieved service: {Name} available at {Centers} centers",
                service.NameEn,
                service.ServiceCenters?.Count ?? 0);

            return service;
        }
    }
}