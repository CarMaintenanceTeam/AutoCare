using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.ServiceCenters.Models;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetAllServiceCenters
{
    /// <summary>
    /// Query to get all service centers with filtering and pagination
    /// Implements CQRS Query pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves service centers
    /// - Immutability: Record type
    /// - Query Pattern: Read-only operation
    /// 
    /// Features:
    /// - Pagination support
    /// - City filtering
    /// - Active status filtering
    /// - Sorting by name or distance
    /// - Service filtering (centers offering specific service)
    /// </summary>
    /// <param name="PageNumber">Page number (1-based)</param>
    /// <param name="PageSize">Items per page</param>
    /// <param name="City">Filter by city (optional)</param>
    /// <param name="IsActive">Filter by active status (optional, default: true)</param>
    /// <param name="ServiceId">Filter by service offered (optional)</param>
    /// <param name="SortBy">Sort field (Name, Distance)</param>
    /// <param name="SortOrder">Sort order (Asc, Desc)</param>
    public sealed record GetAllServiceCentersQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? City = null,
        bool? IsActive = true,
        int? ServiceId = null,
        string SortBy = "Name",
        string SortOrder = "Asc"
    ) : IQuery<PaginatedList<ServiceCenterListDto>>;
}