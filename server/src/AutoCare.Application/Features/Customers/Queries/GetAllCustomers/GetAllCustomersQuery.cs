using System;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Customers.Models;

namespace AutoCare.Application.Features.Customers.Queries.GetAllCustomers
{
    /// <summary>
    /// Admin query to get all customers with pagination and optional filters.
    /// </summary>
    /// <param name="PageNumber">Page number</param>
    /// <param name="PageSize">Page size</param>
    /// <param name="Search">Search term for name/email (optional)</param>
    /// <param name="City">Filter by city (optional)</param>
    /// <param name="IsActive">Filter by active status (optional)</param>
    /// <param name="SortBy">Sort field (Name, CreatedAt)</param>
    /// <param name="SortOrder">Sort order (Asc, Desc)</param>
    public sealed record GetAllCustomersQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? Search = null,
        string? City = null,
        bool? IsActive = null,
        string SortBy = "Name",
        string SortOrder = "Asc"
    ) : IQuery<PaginatedList<CustomerListDto>>;
}