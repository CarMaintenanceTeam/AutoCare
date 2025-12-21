using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Services.Models;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Services.Queries.GetAllServices
{
    /// <summary>
    /// Handler for GetAllServicesQuery
    /// Implements Query Handler pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves services
    /// - Performance: Uses projection for efficient queries
    /// - Flexibility: Multiple filtering options
    /// </summary>
    public sealed class GetAllServicesQueryHandler
        : IQueryHandler<GetAllServicesQuery, PaginatedList<ServiceListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetAllServicesQueryHandler> _logger;

        public GetAllServicesQueryHandler(
            IApplicationDbContext context,
            ILogger<GetAllServicesQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the query to get all services
        /// </summary>
        /// <param name="request">Query with filters and pagination</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of services</returns>
        public async Task<PaginatedList<ServiceListDto>> Handle(
            GetAllServicesQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving services. Page: {PageNumber}, Size: {PageSize}, Type: {Type}, Active: {Active}",
                request.PageNumber,
                request.PageSize,
                request.ServiceType ?? "All",
                request.IsActive?.ToString() ?? "All");

            // Build query with filters
            var query = BuildQuery(request);

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortOrder);

            // Project to DTO
            var projectedQuery = query.Select(s => new ServiceListDto
            {
                Id = s.Id,
                NameEn = s.NameEn,
                NameAr = s.NameAr,
                BasePrice = s.BasePrice,
                EstimatedDurationMinutes = s.EstimatedDurationMinutes,
                ServiceType = s.ServiceType.ToString(),
                IsActive = s.IsActive,
                AvailableAt = s.ServiceCenterServices.Count(scs => scs.IsAvailable)
            });

            // Create pagination parameters
            var pagination = new PaginationParams(request.PageNumber, request.PageSize);

            // Execute query with pagination
            var result = await projectedQuery.ToPaginatedListAsync(pagination, cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} services out of {Total}",
                result.Items.Count,
                result.TotalCount);

            return result;
        }

        /// <summary>
        /// Builds the base query with filters
        /// </summary>
        /// <param name="request">Query request</param>
        /// <returns>Filtered queryable</returns>
        private IQueryable<Domain.Entities.Service> BuildQuery(GetAllServicesQuery request)
        {
            var query = _context.Services
                .Include(s => s.ServiceCenterServices)
                .AsQueryable();

            // Filter by active status
            if (request.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == request.IsActive.Value);
            }

            // Filter by service type
            if (!string.IsNullOrWhiteSpace(request.ServiceType))
            {
                if (Enum.TryParse<ServiceType>(request.ServiceType, true, out var serviceType))
                {
                    query = query.Where(s => s.ServiceType == serviceType);
                }
            }

            // Filter by price range
            if (request.MinPrice.HasValue)
            {
                query = query.Where(s => s.BasePrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(s => s.BasePrice <= request.MaxPrice.Value);
            }

            return query;
        }

        /// <summary>
        /// Applies sorting to query
        /// </summary>
        /// <param name="query">Source query</param>
        /// <param name="sortBy">Sort field</param>
        /// <param name="sortOrder">Sort order (Asc/Desc)</param>
        /// <returns>Sorted queryable</returns>
        private IQueryable<Domain.Entities.Service> ApplySorting(
            IQueryable<Domain.Entities.Service> query,
            string sortBy,
            string sortOrder)
        {
            var isAscending = sortOrder.Equals("Asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "name" => isAscending
                    ? query.OrderBy(s => s.NameEn)
                    : query.OrderByDescending(s => s.NameEn),

                "price" => isAscending
                    ? query.OrderBy(s => s.BasePrice)
                    : query.OrderByDescending(s => s.BasePrice),

                "duration" => isAscending
                    ? query.OrderBy(s => s.EstimatedDurationMinutes)
                    : query.OrderByDescending(s => s.EstimatedDurationMinutes),

                _ => query.OrderBy(s => s.NameEn) // Default sort
            };
        }
    }
}