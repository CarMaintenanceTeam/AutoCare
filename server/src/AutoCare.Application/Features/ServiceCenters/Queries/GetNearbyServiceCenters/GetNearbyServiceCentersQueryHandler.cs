using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.ServiceCenters.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetNearbyServiceCenters
{
    /// <summary>
    /// Handler for GetNearbyServiceCentersQuery
    /// Uses Haversine formula to calculate distances from user location
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves nearby service centers
    /// - Performance: In-memory distance calculation after filtering
    /// - Accuracy: Uses Haversine formula for spherical distance
    /// 
    /// Haversine Formula:
    /// Calculates great-circle distances between two points on a sphere
    /// given their longitudes and latitudes
    /// Accuracy: Within 0.5% for distances up to 500km
    /// 
    /// Performance Optimization:
    /// 1. Filter by lat/lng bounding box first (uses indexes)
    /// 2. Calculate exact distance in memory
    /// 3. Filter by radius
    /// 4. Sort by distance
    /// 5. Apply pagination
    /// </summary>
    public sealed class GetNearbyServiceCentersQueryHandler
        : IQueryHandler<GetNearbyServiceCentersQuery, PaginatedList<ServiceCenterListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetNearbyServiceCentersQueryHandler> _logger;

        // Earth's radius in kilometers
        private const double EarthRadiusKm = 6371.0;

        public GetNearbyServiceCentersQueryHandler(
            IApplicationDbContext context,
            ILogger<GetNearbyServiceCentersQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the query to get nearby service centers
        /// </summary>
        /// <param name="request">Query with location and filters</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Paginated list of nearby service centers sorted by distance</returns>
        public async Task<PaginatedList<ServiceCenterListDto>> Handle(
            GetNearbyServiceCentersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Searching for service centers near location ({Latitude}, {Longitude}) within {Radius}km",
                request.Latitude,
                request.Longitude,
                request.RadiusKm);

            // Calculate bounding box for preliminary filtering
            var boundingBox = CalculateBoundingBox(
                request.Latitude,
                request.Longitude,
                request.RadiusKm);

            // Build query with bounding box filter (uses indexes)
            var query = BuildQuery(request, boundingBox);

            // Load service centers (still need to calculate exact distance)
            var serviceCenters = await query.ToListAsync(cancellationToken);

            _logger.LogDebug(
                "Retrieved {Count} service centers within bounding box",
                serviceCenters.Count);

            // Calculate exact distances and filter by radius
            var serviceCentersWithDistance = serviceCenters
                .Select(sc => new
                {
                    ServiceCenter = sc,
                    Distance = CalculateDistance(
                        request.Latitude,
                        request.Longitude,
                        sc.Latitude,
                        sc.Longitude)
                })
                .Where(x => x.Distance <= request.RadiusKm)
                .OrderBy(x => x.Distance)
                .ToList();

            _logger.LogInformation(
                "Found {Count} service centers within {Radius}km radius",
                serviceCentersWithDistance.Count,
                request.RadiusKm);

            // Project to DTO with distance
            var dtos = serviceCentersWithDistance
                .Select(x => new ServiceCenterListDto
                {
                    Id = x.ServiceCenter.Id,
                    NameEn = x.ServiceCenter.NameEn,
                    NameAr = x.ServiceCenter.NameAr,
                    City = x.ServiceCenter.City,
                    PhoneNumber = x.ServiceCenter.PhoneNumber,
                    WorkingHours = x.ServiceCenter.WorkingHours,
                    Latitude = x.ServiceCenter.Latitude,
                    Longitude = x.ServiceCenter.Longitude,
                    IsActive = x.ServiceCenter.IsActive,
                    ServicesCount = x.ServiceCenter.ServiceCenterServices.Count(scs => scs.IsAvailable),
                    Distance = Math.Round(x.Distance, 2) // Round to 2 decimal places
                })
                .ToList();

            // Apply pagination
            var totalCount = dtos.Count;
            var pagination = new PaginationParams(request.PageNumber, request.PageSize);
            var paginatedItems = dtos
                .Skip(pagination.Skip)
                .Take(pagination.Take)
                .ToList();

            var result = PaginatedList<ServiceCenterListDto>.Create(
                paginatedItems,
                totalCount,
                pagination);

            _logger.LogInformation(
                "Returning {Count} service centers on page {Page}",
                result.Items.Count,
                request.PageNumber);

            return result;
        }

        /// <summary>
        /// Builds the query with filters
        /// </summary>
        private IQueryable<Domain.Entities.ServiceCenter> BuildQuery(
            GetNearbyServiceCentersQuery request,
            BoundingBox boundingBox)
        {
            var query = _context.ServiceCenters
                .Include(sc => sc.ServiceCenterServices)
                .Where(sc => sc.IsActive) // Only active centers
                .Where(sc =>
                    sc.Latitude >= boundingBox.MinLatitude &&
                    sc.Latitude <= boundingBox.MaxLatitude &&
                    sc.Longitude >= boundingBox.MinLongitude &&
                    sc.Longitude <= boundingBox.MaxLongitude);

            // Filter by service offered
            if (request.ServiceId.HasValue)
            {
                query = query.Where(sc => sc.ServiceCenterServices
                    .Any(scs => scs.ServiceId == request.ServiceId.Value && scs.IsAvailable));
            }

            return query;
        }

        /// <summary>
        /// Calculates bounding box for preliminary filtering
        /// Uses approximate degrees per kilometer
        /// </summary>
        /// <param name="latitude">Center latitude</param>
        /// <param name="longitude">Center longitude</param>
        /// <param name="radiusKm">Radius in kilometers</param>
        /// <returns>Bounding box coordinates</returns>
        private BoundingBox CalculateBoundingBox(decimal latitude, decimal longitude, int radiusKm)
        {
            // Approximate degrees per kilometer
            // 1 degree latitude ≈ 111 km (constant)
            // 1 degree longitude ≈ 111 km * cos(latitude)

            var latDegrees = (decimal)(radiusKm / 111.0);
            var lonDegrees = (decimal)(radiusKm / (111.0 * Math.Cos(ToRadians((double)latitude))));

            return new BoundingBox
            {
                MinLatitude = latitude - latDegrees,
                MaxLatitude = latitude + latDegrees,
                MinLongitude = longitude - lonDegrees,
                MaxLongitude = longitude + lonDegrees
            };
        }

        /// <summary>
        /// Calculates distance between two points using Haversine formula
        /// Returns distance in kilometers
        /// </summary>
        /// <param name="lat1">Latitude of first point</param>
        /// <param name="lon1">Longitude of first point</param>
        /// <param name="lat2">Latitude of second point</param>
        /// <param name="lon2">Longitude of second point</param>
        /// <returns>Distance in kilometers</returns>
        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) *
                    Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180.0);
        }

        /// <summary>
        /// Bounding box for preliminary filtering
        /// </summary>
        private sealed class BoundingBox
        {
            public decimal MinLatitude { get; init; }
            public decimal MaxLatitude { get; init; }
            public decimal MinLongitude { get; init; }
            public decimal MaxLongitude { get; init; }
        }
    }
}