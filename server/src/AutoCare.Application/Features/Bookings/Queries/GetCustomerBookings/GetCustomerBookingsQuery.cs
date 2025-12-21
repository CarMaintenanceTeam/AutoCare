using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Bookings.Models;

namespace AutoCare.Application.Features.Bookings.Queries.GetCustomerBookings
{
    /// <summary>
    /// Query to get bookings for the authenticated customer
    /// 
    /// Features:
    /// - Pagination support
    /// - Status filtering
    /// - Date range filtering
    /// - Sorting options
    /// </summary>
    /// <param name="PageNumber">Page number</param>
    /// <param name="PageSize">Page size</param>
    /// <param name="Status">Filter by status (optional)</param>
    /// <param name="FromDate">Filter by date from (optional)</param>
    /// <param name="ToDate">Filter by date to (optional)</param>
    /// <param name="SortBy">Sort field (Date, CreatedAt)</param>
    /// <param name="SortOrder">Sort order (Asc, Desc)</param>
    public sealed record GetCustomerBookingsQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? Status = null,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        string SortBy = "Date",
        string SortOrder = "Desc"
    ) : IQuery<PaginatedList<BookingListDto>>;
}