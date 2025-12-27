using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Customers.Models;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Customers.Queries.GetAllCustomers
{
    /// <summary>
    /// Handler for GetAllCustomersQuery.
    /// Admin / manager employees can view all customers.
    /// </summary>
    public sealed class GetAllCustomersQueryHandler
        : IQueryHandler<GetAllCustomersQuery, PaginatedList<CustomerListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<GetAllCustomersQueryHandler> _logger;

        public GetAllCustomersQueryHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            ILogger<GetAllCustomersQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaginatedList<CustomerListDto>> Handle(
            GetAllCustomersQuery request,
            CancellationToken cancellationToken)
        {
            // Authorization: Admin OR Employee with Manager/Owner role
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to view customers.");

            var userType = _currentUserService.UserType;
            var isAdmin = userType != null &&
                          userType.Equals(UserType.Admin.ToString(), StringComparison.OrdinalIgnoreCase);

            var isManager = false;
            if (!isAdmin && userType != null &&
                userType.Equals(UserType.Employee.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                // Check employee role (Manager or Owner)
                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == userId, cancellationToken);

                if (employee != null &&
                    (employee.Role == EmployeeRole.Manager || employee.Role == EmployeeRole.Owner))
                {
                    isManager = true;
                }
            }

            if (!isAdmin && !isManager)
            {
                throw new ForbiddenException("You do not have permission to view customers.");
            }

            _logger.LogInformation(
                "Retrieving customers for admin overview. Requested by UserId: {UserId}, UserType: {UserType}",
                userId,
                userType);

            var query = from c in _context.Customers
                        join u in _context.Users on c.UserId equals u.Id
                        select new { c, u };

            // Filters
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(x =>
                    x.u.FullName.ToLower().Contains(search) ||
                    x.u.Email.ToLower().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(request.City))
            {
                var city = request.City.ToLower();
                query = query.Where(x => x.c.City != null && x.c.City.ToLower().Contains(city));
            }

            if (request.IsActive.HasValue)
            {
                query = query.Where(x => x.u.IsActive == request.IsActive.Value);
            }

            // Project to DTO with counts
            var projected = query.Select(x => new CustomerListDto
            {
                Id = x.c.Id,
                UserId = x.u.Id,
                FullName = x.u.FullName,
                Email = x.u.Email,
                PhoneNumber = x.u.PhoneNumber,
                City = x.c.City,
                Address = x.c.Address,
                IsActive = x.u.IsActive,
                VehiclesCount = _context.Vehicles.Count(v => v.CustomerId == x.c.Id),
                BookingsCount = _context.Bookings.Count(b => b.CustomerId == x.c.Id),
                CreatedAt = x.c.CreatedAt
            });

            // Sorting
            var isAscending = request.SortOrder.Equals("Asc", StringComparison.OrdinalIgnoreCase);
            projected = request.SortBy.ToLower() switch
            {
                "name" => isAscending
                    ? projected.OrderBy(c => c.FullName)
                    : projected.OrderByDescending(c => c.FullName),

                "createdat" => isAscending
                    ? projected.OrderBy(c => c.CreatedAt)
                    : projected.OrderByDescending(c => c.CreatedAt),

                _ => projected.OrderBy(c => c.FullName)
            };

            var pagination = new PaginationParams(request.PageNumber, request.PageSize);
            var result = await projected.ToPaginatedListAsync(pagination, cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} customers out of {Total}",
                result.Items.Count,
                result.TotalCount);

            return result;
        }
    }
}