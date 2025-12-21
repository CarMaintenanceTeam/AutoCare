using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.ServiceCenters.Queries.GetServiceCenterById
{
    /// <summary>
    /// Validator for GetServiceCenterByIdQuery
    /// Validates service center ID
    /// </summary>
    public sealed class GetServiceCenterByIdQueryValidator : AbstractValidator<GetServiceCenterByIdQuery>
    {
        public GetServiceCenterByIdQueryValidator()
        {
            // ServiceCenterId validation
            RuleFor(x => x.ServiceCenterId)
                .GreaterThan(0)
                .WithMessage("Service Center ID must be greater than 0");
        }
    }
}