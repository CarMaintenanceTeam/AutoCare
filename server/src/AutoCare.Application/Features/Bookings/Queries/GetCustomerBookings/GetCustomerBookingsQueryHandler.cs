using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Bookings.Models;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Bookings.Queries.GetCustomerBookings
{
    /// <summary>
    /// Handler for GetCustomerBookingsQuery
    /// Authorization: Customers can only view their own bookings
    /// </summary>
    public sealed class GetCustomerBookingsQueryHandler
        : IQueryHandler<GetCustomerBookingsQuery, PaginatedList<BookingListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetCustomerBookingsQueryHandler> _logger;

        public GetCustomerBookingsQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<GetCustomerBookingsQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaginatedList<BookingListDto>> Handle(
            GetCustomerBookingsQuery request,
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to view bookings");

            _logger.LogInformation("Retrieving bookings for UserId: {UserId}", userId);

            // Get customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (customer == null)
            {
                throw new NotFoundException("Customer profile not found");
            }

            // Build query
            var query = BuildQuery(request, customer.Id);

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortOrder);

            // Project to DTO with service price calculation (... not ideal, but EF Core limitations)
            // var projectedQuery = query.Select(b => new BookingListDto
            // {
            //     Id = b.Id,
            //     BookingNumber = b.BookingNumber,
            //     VehicleInfo = $"{b.Vehicle.Brand} {b.Vehicle.Model} ({b.Vehicle.Year})",
            //     ServiceCenterName = b.ServiceCenter.NameEn,
            //     ServiceName = b.Service.NameEn,
            //     BookingDate = b.BookingDate,
            //     BookingTime = b.BookingTime,
            //     Status = b.Status.ToString(),
            //     ServicePrice = b.Service.ServiceCenterServices
            //         .Where(scs => scs.ServiceId == b.ServiceId && scs.ServiceCenterId == b.ServiceCenterId)
            //         .Select(scs => scs.CustomPrice ?? b.Service.BasePrice)
            //         .FirstOrDefault(),
            //     CreatedAt = b.CreatedAt
            // });
            // Alternative approach to avoid subquery in projection (Left Join) (... still not ideal)
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
            // Final approach using navigation property to avoid subquery in projection
            // var projectedQuery = query.Select(b => new BookingListDto
            // {
            //     Id = b.Id,
            //     BookingNumber = b.BookingNumber,
            //     VehicleInfo = $"{b.Vehicle.Brand} {b.Vehicle.Model} ({b.Vehicle.Year})",
            //     ServiceCenterName = b.ServiceCenter.NameEn,
            //     ServiceName = b.Service.NameEn,
            //     BookingDate = b.BookingDate,
            //     BookingTime = b.BookingTime,
            //     Status = b.Status.ToString(),
            //     ServicePrice = b.ServiceCenterService != null && b.ServiceCenterService.CustomPrice.HasValue
            //         ? (b.ServiceCenterService.CustomPrice ?? b.Service.BasePrice)
            //         : b.Service.BasePrice,
            //     CreatedAt = b.CreatedAt
            // });

            // Execute with pagination
            var pagination = new PaginationParams(request.PageNumber, request.PageSize);
            var result = await projectedQuery.ToPaginatedListAsync(pagination, cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} bookings out of {Total}",
                result.Items.Count,
                result.TotalCount);

            return result;
        }

        private IQueryable<Domain.Entities.Booking> BuildQuery(
            GetCustomerBookingsQuery request,
            int customerId)
        {
            var query = _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.ServiceCenter)
                .Include(b => b.Service)
                .Where(b => b.CustomerId == customerId);

            // Filter by status
            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<BookingStatus>(request.Status, true, out var status))
            {
                query = query.Where(b => b.Status == status);
            }

            // Filter by date range
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