using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Bookings.Queries.GetCustomerBookings
{
    /// <summary>
    /// Validator for GetCustomerBookingsQuery
    /// </summary>
    public sealed class GetCustomerBookingsQueryValidator : AbstractValidator<GetCustomerBookingsQuery>
    {
        private static readonly string[] ValidStatuses =
            { "Pending", "Confirmed", "InProgress", "Completed", "Cancelled" };
        private static readonly string[] ValidSortFields = { "Date", "CreatedAt", "Status" };
        private static readonly string[] ValidSortOrders = { "Asc", "Desc" };

        public GetCustomerBookingsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must not exceed 100");

            RuleFor(x => x.Status)
                .Must(status => ValidStatuses.Contains(status!, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}")
                .When(x => !string.IsNullOrWhiteSpace(x.Status));

            RuleFor(x => x.SortBy)
                .Must(sortBy => ValidSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Sort by must be one of: {string.Join(", ", ValidSortFields)}");

            RuleFor(x => x.SortOrder)
                .Must(order => ValidSortOrders.Contains(order, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Sort order must be one of: {string.Join(", ", ValidSortOrders)}");

            RuleFor(x => x)
                .Must(x => !x.FromDate.HasValue || !x.ToDate.HasValue || x.ToDate >= x.FromDate)
                .WithMessage("ToDate must be greater than or equal to FromDate")
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
        }
    }
}