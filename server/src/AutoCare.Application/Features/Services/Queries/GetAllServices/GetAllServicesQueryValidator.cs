using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Services.Queries.GetAllServices
{
    /// <summary>
    /// Validator for GetAllServicesQuery
    /// Validates pagination and filtering parameters
    /// </summary>
    public sealed class GetAllServicesQueryValidator : AbstractValidator<GetAllServicesQuery>
    {
        private static readonly string[] ValidServiceTypes = { "Maintenance", "SpareParts" };
        private static readonly string[] ValidSortFields = { "Name", "Price", "Duration" };
        private static readonly string[] ValidSortOrders = { "Asc", "Desc" };

        public GetAllServicesQueryValidator()
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

            // ServiceType validation
            RuleFor(x => x.ServiceType)
                .Must(type => ValidServiceTypes.Contains(type!, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Service type must be one of: {string.Join(", ", ValidServiceTypes)}")
                .When(x => !string.IsNullOrWhiteSpace(x.ServiceType));

            // Price range validation
            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum price must be greater than or equal to 0")
                .When(x => x.MinPrice.HasValue);

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Maximum price must be greater than or equal to 0")
                .When(x => x.MaxPrice.HasValue);

            // Validate max price >= min price
            RuleFor(x => x)
                .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MaxPrice >= x.MinPrice)
                .WithMessage("Maximum price must be greater than or equal to minimum price")
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue);

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