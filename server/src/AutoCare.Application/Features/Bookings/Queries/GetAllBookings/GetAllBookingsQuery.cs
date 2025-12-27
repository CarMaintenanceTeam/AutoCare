using System;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Bookings.Models;

namespace AutoCare.Application.Features.Bookings.Queries.GetAllBookings
{
    /// <summary>
    /// Admin/staff query to get bookings for all customers.
    /// Supports pagination, status filtering, date range filtering and sorting.
    /// </summary>
    /// <param name="PageNumber">Page number</param>
    /// <param name="PageSize">Page size</param>
    /// <param name="Status">Filter by status (optional)</param>
    /// <param name="FromDate">Filter by date from (optional)</param>
    /// <param name="ToDate">Filter by date to (optional)</param>
    /// <param name="SortBy">Sort field (Date, CreatedAt, Status)</param>
    /// <param name="SortOrder">Sort order (Asc, Desc)</param>
    public sealed record GetAllBookingsQuery(
        int PageNumber = 1,
        int PageSize = 10,
        string? Status = null,
        DateTime? FromDate = null,
        DateTime? ToDate = null,
        string SortBy = "Date",
        string SortOrder = "Desc"
    ) : IQuery<PaginatedList<BookingListDto>>;
}