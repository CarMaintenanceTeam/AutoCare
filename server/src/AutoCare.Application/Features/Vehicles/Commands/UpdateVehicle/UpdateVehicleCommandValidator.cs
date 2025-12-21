using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Vehicles.Commands.UpdateVehicle
{
    /// <summary>
    /// Validator for UpdateVehicleCommand
    /// Validates update input
    /// </summary>
    public sealed class UpdateVehicleCommandValidator : AbstractValidator<UpdateVehicleCommand>
    {
        public UpdateVehicleCommandValidator()
        {
            // VehicleId validation
            RuleFor(x => x.VehicleId)
                .GreaterThan(0)
                .WithMessage("Vehicle ID must be greater than 0");

            // Brand validation
            RuleFor(x => x.Brand)
                .NotEmpty()
                    .WithMessage("Brand is required")
                .MinimumLength(2)
                    .WithMessage("Brand must be at least 2 characters")
                .MaximumLength(100)
                    .WithMessage("Brand must not exceed 100 characters")
                .Matches(@"^[a-zA-Z0-9\s\-]+$")
                    .WithMessage("Brand must contain only letters, numbers, spaces, and hyphens");

            // Model validation
            RuleFor(x => x.Model)
                .NotEmpty()
                    .WithMessage("Model is required")
                .MinimumLength(1)
                    .WithMessage("Model must be at least 1 character")
                .MaximumLength(100)
                    .WithMessage("Model must not exceed 100 characters");

            // Year validation
            RuleFor(x => x.Year)
                .GreaterThanOrEqualTo(1900)
                    .WithMessage("Year must be 1900 or later")
                .LessThanOrEqualTo(DateTime.UtcNow.Year + 1)
                    .WithMessage($"Year cannot exceed {DateTime.UtcNow.Year + 1}");

            // Color validation (optional)
            RuleFor(x => x.Color)
                .MaximumLength(50)
                    .WithMessage("Color must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Color));
        }
    }
}