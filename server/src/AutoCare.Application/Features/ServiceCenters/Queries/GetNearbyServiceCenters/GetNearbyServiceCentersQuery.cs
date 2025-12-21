using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.ServiceCenters.Models;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetNearbyServiceCenters
{
    /// <summary>
    /// Query to get nearby service centers based on GPS location
    /// Uses Haversine formula for distance calculation
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves nearby service centers
    /// - Immutability: Record type
    /// - Query Pattern: Read-only operation
    /// 
    /// Features:
    /// - Distance-based filtering
    /// - GPS coordinate validation
    /// - Sorted by distance (nearest first)
    /// - Optional service filtering
    /// - Pagination support
    /// </summary>
    /// <param name="Latitude">User's latitude (-90 to 90)</param>
    /// <param name="Longitude">User's longitude (-180 to 180)</param>
    /// <param name="RadiusKm">Search radius in kilometers (default: 50km, max: 200km)</param>
    /// <param name="PageNumber">Page number (1-based)</param>
    /// <param name="PageSize">Items per page</param>
    /// <param name="ServiceId">Filter by service offered (optional)</param>
    public sealed record GetNearbyServiceCentersQuery(
        decimal Latitude,
        decimal Longitude,
        int RadiusKm = 50,
        int PageNumber = 1,
        int PageSize = 10,
        int? ServiceId = null
    ) : IQuery<PaginatedList<ServiceCenterListDto>>;
}