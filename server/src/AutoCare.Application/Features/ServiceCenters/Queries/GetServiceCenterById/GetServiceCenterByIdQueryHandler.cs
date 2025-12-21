using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.ServiceCenters.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetServiceCenterById
{
    /// <summary>
    /// Handler for GetServiceCenterByIdQuery
    /// Retrieves complete service center details including services
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves one service center
    /// - Fail Fast: Throws NotFoundException if not found
    /// - Performance: Uses projection for efficient queries
    /// </summary>
    public sealed class GetServiceCenterByIdQueryHandler(
        IApplicationDbContext context,
        ILogger<GetServiceCenterByIdQueryHandler> logger)
                : IQueryHandler<GetServiceCenterByIdQuery, ServiceCenterDto>
    {
        private readonly IApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ILogger<GetServiceCenterByIdQueryHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Handles the query to get service center by ID
        /// </summary>
        /// <param name="request">Query with service center ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Service center details</returns>
        /// <exception cref="NotFoundException">Thrown when service center not found</exception>
        public async Task<ServiceCenterDto> Handle(
            GetServiceCenterByIdQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving service center with ID: {ServiceCenterId}",
                request.ServiceCenterId);

            // Query with projection for performance
            var serviceCenter = await _context.ServiceCenters
                .Where(sc => sc.Id == request.ServiceCenterId)
                .Select(sc => new ServiceCenterDto
                {
                    Id = sc.Id,
                    NameEn = sc.NameEn,
                    NameAr = sc.NameAr,
                    AddressEn = sc.AddressEn,
                    AddressAr = sc.AddressAr,
                    City = sc.City,
                    Latitude = sc.Latitude,
                    Longitude = sc.Longitude,
                    PhoneNumber = sc.PhoneNumber,
                    Email = sc.Email,
                    WorkingHours = sc.WorkingHours,
                    IsActive = sc.IsActive,
                    CreatedAt = sc.CreatedAt,
                    UpdatedAt = sc.UpdatedAt,
                    Services = sc.ServiceCenterServices
                        .Where(scs => scs.Service.IsActive) // Only active services
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
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (serviceCenter == null)
            {
                _logger.LogWarning(
                    "Service center not found with ID: {ServiceCenterId}",
                    request.ServiceCenterId);

                throw new NotFoundException("ServiceCenter", request.ServiceCenterId);
            }

            _logger.LogInformation(
                "Retrieved service center: {Name} with {ServicesCount} services",
                serviceCenter.NameEn,
                serviceCenter.Services.Count);

            return serviceCenter;
        }
    }
}