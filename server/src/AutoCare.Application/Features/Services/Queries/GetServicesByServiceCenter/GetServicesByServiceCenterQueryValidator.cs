using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Services.Queries.GetServicesByServiceCenter
{
    /// <summary>
    /// Validator for GetServicesByServiceCenterQuery
    /// Validates service center ID and filters
    /// </summary>
    public sealed class GetServicesByServiceCenterQueryValidator
        : AbstractValidator<GetServicesByServiceCenterQuery>
    {
        private static readonly string[] ValidServiceTypes = ["Maintenance", "SpareParts"];

        public GetServicesByServiceCenterQueryValidator()
        {
            RuleFor(x => x.ServiceCenterId)
                .GreaterThan(0)
                .WithMessage("Service center ID must be greater than 0");

            RuleFor(x => x.ServiceType)
                .Must(type => ValidServiceTypes.Contains(type!, StringComparer.OrdinalIgnoreCase))
                .WithMessage($"Service type must be one of: {string.Join(", ", ValidServiceTypes)}")
                .When(x => !string.IsNullOrWhiteSpace(x.ServiceType));
        }
    }
}