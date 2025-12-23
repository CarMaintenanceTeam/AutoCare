using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.ServiceCenters.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetAllServiceCenters
{
    /// <summary>
    /// Handler for GetAllServiceCentersQuery
    /// Implements Query Handler pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves service centers
    /// - Dependency Inversion: Depends on abstractions
    /// - Performance: Uses projection and efficient queries
    /// 
    /// Query Optimization:
    /// - Select only needed fields (projection)
    /// - Efficient filtering with indexes
    /// - Deferred execution with IQueryable
    /// - Batch loading with Include
    /// </summary>
    public sealed class GetAllServiceCentersQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAllServiceCentersQueryHandler> logger)
                : IQueryHandler<GetAllServiceCentersQuery, PaginatedList<ServiceCenterListDto>>
    {

        private readonly ILogger<GetAllServiceCentersQueryHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Handles the query to get all service centers
        /// </summary>
        /// <param name="request">Query with filters and pagination</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of service centers</returns>
        public async Task<PaginatedList<ServiceCenterListDto>> Handle(
            GetAllServiceCentersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Retrieving service centers. Page: {PageNumber}, Size: {PageSize}, City: {City}, Active: {IsActive}",
                request.PageNumber,
                request.PageSize,
                request.City ?? "All",
                request.IsActive?.ToString() ?? "All");

            // Build query with filters
            var query = BuildQuery(request);

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortOrder);

            // Project to DTO
            var projectedQuery = query.Select(sc => new ServiceCenterListDto
            {
                Id = sc.Id,
                NameEn = sc.NameEn,
                NameAr = sc.NameAr,
                City = sc.City,
                PhoneNumber = sc.PhoneNumber,
                WorkingHours = sc.WorkingHours,
                Latitude = sc.Latitude,
                Longitude = sc.Longitude,
                IsActive = sc.IsActive,
                ServicesCount = sc.ServiceCenterServices.Count(scs => scs.IsAvailable),
                Distance = null // Will be calculated if location provided
            });

            // Create pagination parameters
            var pagination = new PaginationParams(request.PageNumber, request.PageSize);

            // Execute query with pagination
            var result = await projectedQuery.ToPaginatedListAsync(pagination, cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} service centers out of {Total}",
                result.Items.Count,
                result.TotalCount);

            return result;
        }

        /// <summary>
        /// Builds the base query with filters
        /// </summary>
        /// <param name="request">Query request</param>
        /// <returns>Filtered queryable</returns>
        private IQueryable<Domain.Entities.ServiceCenter> BuildQuery(GetAllServiceCentersQuery request)
        {
            var query = _context.ServiceCenters
                .Include(sc => sc.ServiceCenterServices)
                .AsQueryable();

            // Filter by active status
            if (request.IsActive.HasValue)
            {
                query = query.Where(sc => sc.IsActive == request.IsActive.Value);
            }

            // Filter by city
            if (!string.IsNullOrWhiteSpace(request.City))
            {
                query = query.Where(sc => sc.City.ToLower() == request.City.ToLower());
            }

            // Filter by service offered
            if (request.ServiceId.HasValue)
            {
                query = query.Where(sc => sc.ServiceCenterServices
                    .Any(scs => scs.ServiceId == request.ServiceId.Value && scs.IsAvailable));
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
        private IQueryable<Domain.Entities.ServiceCenter> ApplySorting(
            IQueryable<Domain.Entities.ServiceCenter> query,
            string sortBy,
            string sortOrder)
        {
            var isAscending = sortOrder.Equals("Asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "name" => isAscending
                    ? query.OrderBy(sc => sc.NameEn)
                    : query.OrderByDescending(sc => sc.NameEn),

                "city" => isAscending
                    ? query.OrderBy(sc => sc.City)
                    : query.OrderByDescending(sc => sc.City),

                _ => query.OrderBy(sc => sc.NameEn) // Default sort
            };
        }
    }
}