using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Vehicles.Commands.AddVehicle
{
    /// <summary>
    /// Validator for AddVehicleCommand
    /// Implements validation rules for vehicle creation
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only validates vehicle input
    /// - Explicit Rules: Clear validation messages
    /// - Domain Knowledge: Enforces business constraints
    /// </summary>
    public sealed class AddVehicleCommandValidator : AbstractValidator<AddVehicleCommand>
    {
        public AddVehicleCommandValidator()
        {
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

            // Plate number validation
            RuleFor(x => x.PlateNumber)
                .NotEmpty()
                    .WithMessage("Plate number is required")
                .MinimumLength(3)
                    .WithMessage("Plate number must be at least 3 characters")
                .MaximumLength(50)
                    .WithMessage("Plate number must not exceed 50 characters");

            // VIN validation (optional but must be valid if provided)
            RuleFor(x => x.VIN)
                .Length(17)
                    .WithMessage("VIN must be exactly 17 characters")
                .Matches(@"^[A-HJ-NPR-Z0-9]+$")
                    .WithMessage("VIN must not contain letters I, O, or Q")
                .When(x => !string.IsNullOrWhiteSpace(x.VIN));

            // Color validation (optional)
            RuleFor(x => x.Color)
                .MaximumLength(50)
                    .WithMessage("Color must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Color));
        }
    }
}