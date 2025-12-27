using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Bookings.Models;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Bookings.Queries.GetAllBookings
{
    /// <summary>
    /// Handler for GetAllBookingsQuery.
    /// Allows Admin/Employee users to retrieve bookings for all customers
    /// with pagination, status/date filters and sorting.
    /// </summary>
    public sealed class GetAllBookingsQueryHandler
        : IQueryHandler<GetAllBookingsQuery, PaginatedList<BookingListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAllBookingsQueryHandler> _logger;

        public GetAllBookingsQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<GetAllBookingsQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaginatedList<BookingListDto>> Handle(
            GetAllBookingsQuery request,
            CancellationToken cancellationToken)
        {
            // Authorization: only Admin/Employee can view all bookings
            var userType = _currentUserService.UserType;
            if (string.IsNullOrWhiteSpace(userType) ||
                (!userType.Equals(UserType.Admin.ToString(), StringComparison.OrdinalIgnoreCase) &&
                 !userType.Equals(UserType.Employee.ToString(), StringComparison.OrdinalIgnoreCase)))
            {
                throw new ForbiddenException("You do not have permission to view all bookings.");
            }

            _logger.LogInformation(
                "Retrieving all bookings. Requested by {UserType} (UserId: {UserId})",
                userType,
                _currentUserService.UserId);

            // Build base query (all bookings)
            var query = BuildQuery(request);

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortOrder);

            // Project to DTO with service price calculation (same approach as customer query)
            var projectedQuery = from b in query
                                 join scs in _context.ServiceCenterServices
                                 on new { b.ServiceCenterId, b.ServiceId }
                                 equals new { scs.ServiceCenterId, scs.ServiceId }
                                 into ServiceCenterServices
                                 from scs in ServiceCenterServices.DefaultIfEmpty()
                                 select new BookingListDto
                                 {
                                     Id = b.Id,
                                     BookingNumber = b.BookingNumber,
                                     VehicleInfo = $"{b.Vehicle.Brand} {b.Vehicle.Model} ({b.Vehicle.Year})",
                                     ServiceCenterName = b.ServiceCenter.NameEn,
                                     ServiceName = b.Service.NameEn,
                                     BookingDate = b.BookingDate,
                                     BookingTime = b.BookingTime,
                                     Status = b.Status.ToString(),
                                     ServicePrice = scs != null && scs.CustomPrice.HasValue
                                         ? (scs.CustomPrice ?? b.Service.BasePrice)
                                         : b.Service.BasePrice,
                                     CreatedAt = b.CreatedAt
                                 };

            var pagination = new PaginationParams(request.PageNumber, request.PageSize);
            var result = await projectedQuery.ToPaginatedListAsync(pagination, cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} bookings out of {Total} for admin view",
                result.Items.Count,
                result.TotalCount);

            return result;
        }

        private IQueryable<Domain.Entities.Booking> BuildQuery(GetAllBookingsQuery request)
        {
            var query = _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.ServiceCenter)
                .Include(b => b.Service)
                .AsQueryable();

            // Filter by status (optional)
            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<BookingStatus>(request.Status, true, out var status))
            {
                query = query.Where(b => b.Status == status);
            }

            // Filter by date range (optional)
            if (request.FromDate.HasValue)
            {
                query = query.Where(b => b.BookingDate >= request.FromDate.Value.Date);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(b => b.BookingDate <= request.ToDate.Value.Date);
            }

            return query;
        }

        private IQueryable<Domain.Entities.Booking> ApplySorting(
            IQueryable<Domain.Entities.Booking> query,
            string sortBy,
            string sortOrder)
        {
            var isAscending = sortOrder.Equals("Asc", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                "date" => isAscending
                    ? query.OrderBy(b => b.BookingDate).ThenBy(b => b.BookingTime)
                    : query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.BookingTime),

                "createdat" => isAscending
                    ? query.OrderBy(b => b.CreatedAt)
                    : query.OrderByDescending(b => b.CreatedAt),

                "status" => isAscending
                    ? query.OrderBy(b => b.Status)
                    : query.OrderByDescending(b => b.Status),

                _ => query.OrderByDescending(b => b.BookingDate).ThenByDescending(b => b.BookingTime)
            };
        }
    }
}