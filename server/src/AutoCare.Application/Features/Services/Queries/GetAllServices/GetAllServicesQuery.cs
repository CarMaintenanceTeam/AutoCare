using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Services.Models;

namespace AutoCare.Application.Features.Services.Queries.GetAllServices
{
    /// <summary>
    /// Query to get all services with filtering and pagination
    /// Implements CQRS Query pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only retrieves services
    /// - Immutability: Record type
    /// - Query Pattern: Read-only operation
    /// 
    /// Features:
    /// - Pagination support
    /// - Service type filtering (Maintenance, SpareParts)
    /// - Active status filtering
    /// - Price range filtering
    /// - Sorting options
    /// </summary>
    /// <param name="PageNumber">Page number (1-based)</param>
    /// <param name="PageSize">Items per page</param>
    /// <param name="ServiceType">Filter by service type (optional)</param>
    /// <param name="IsActive">Filter by active status (optional, default: true)</param>
    /// <param name="MinPrice">Minimum price filter (optional)</param>
    /// <param name="MaxPrice">Maximum price filter (optional)</param>
    /// <param name="SortBy">Sort field (Name, Price, Duration)</param>
    /// <param name="SortOrder">Sort order (Asc, Desc)</param>
    public sealed record GetAllServicesQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? ServiceType = null,
        bool? IsActive = true,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        string SortBy = "Name",
        string SortOrder = "Asc"
    ) : IQuery<PaginatedList<ServiceListDto>>;
}