using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetNearbyServiceCenters
{
    /// <summary>
    /// Validator for GetNearbyServiceCentersQuery
    /// Validates GPS coordinates and search parameters
    /// </summary>
    public sealed class GetNearbyServiceCentersQueryValidator
        : AbstractValidator<GetNearbyServiceCentersQuery>
    {
        public GetNearbyServiceCentersQueryValidator()
        {
            // Latitude validation (-90 to 90)
            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90 degrees");

            // Longitude validation (-180 to 180)
            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180 degrees");

            // Radius validation (1 to 200 km)
            RuleFor(x => x.RadiusKm)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Radius must be at least 1 kilometer")
                .LessThanOrEqualTo(200)
                .WithMessage("Radius must not exceed 200 kilometers");

            // Pagination validation
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be greater than or equal to 1");

            // PageNumber is 1-based
            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page size must be greater than or equal to 1")
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must not exceed 100");

            // ServiceId validation
            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage("Service ID must be greater than 0")
                .When(x => x.ServiceId.HasValue);
        }
    }
}