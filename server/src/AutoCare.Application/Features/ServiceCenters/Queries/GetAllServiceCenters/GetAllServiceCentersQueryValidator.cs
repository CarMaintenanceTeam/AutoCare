using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetAllServiceCenters
{
    /// <summary>
    /// Validator for GetAllServiceCentersQuery
    /// Validates pagination and filtering parameters
    /// </summary>
    public sealed class GetAllServiceCentersQueryValidator : AbstractValidator<GetAllServiceCentersQuery>
    {
        private static readonly string[] ValidSortFields = { "Name", "Distance", "City" };
        private static readonly string[] ValidSortOrders = { "Asc", "Desc" };

        public GetAllServiceCentersQueryValidator()
        {
            // Pagination validation
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must not exceed 100");

            // City validation
            RuleFor(x => x.City)
                .MaximumLength(100)
                .WithMessage("City must not exceed 100 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.City));

            // ServiceId validation
            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage("Service ID must be greater than 0")
                .When(x => x.ServiceId.HasValue);

            // SortBy validation
            RuleFor(x => x.SortBy)
                .Must(sortBy => ValidSortFields.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Sort by must be one of: {string.Join(", ", ValidSortFields)}");

            // SortOrder validation
            RuleFor(x => x.SortOrder)
                .Must(order => ValidSortOrders.Contains(order, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Sort order must be one of: {string.Join(", ", ValidSortOrders)}");
        }
    }
}