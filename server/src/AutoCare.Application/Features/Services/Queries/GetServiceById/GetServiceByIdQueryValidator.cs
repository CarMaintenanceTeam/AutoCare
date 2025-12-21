using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Services.Queries.GetServiceById
{

    /// <summary>
    /// Validator for GetServiceByIdQuery
    /// Validates service ID
    /// </summary>
    public sealed class GetServiceByIdQueryValidator : AbstractValidator<GetServiceByIdQuery>
    {
        public GetServiceByIdQueryValidator()
        {
            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage("Service ID must be greater than 0");
        }
    }
}