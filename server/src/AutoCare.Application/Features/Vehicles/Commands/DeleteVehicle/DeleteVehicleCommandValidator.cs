using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Vehicles.Commands.DeleteVehicle
{
    /// <summary>
    /// Validator for DeleteVehicleCommand
    /// Simple validation for vehicle ID
    /// </summary>
    public sealed class DeleteVehicleCommandValidator : AbstractValidator<DeleteVehicleCommand>
    {
        public DeleteVehicleCommandValidator()
        {
            RuleFor(x => x.VehicleId)
                .GreaterThan(0)
                .WithMessage("Vehicle ID must be greater than 0");
        }
    }
}