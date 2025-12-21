using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Bookings.Commands.CreateBooking
{
    /// <summary>
    /// Validator for CreateBookingCommand
    /// Validates booking creation parameters
    /// </summary>
    public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            // VehicleId validation
            RuleFor(x => x.VehicleId)
                .GreaterThan(0)
                .WithMessage("Vehicle ID must be greater than 0");

            // ServiceCenterId validation
            RuleFor(x => x.ServiceCenterId)
                .GreaterThan(0)
                .WithMessage("Service center ID must be greater than 0");

            // ServiceId validation
            RuleFor(x => x.ServiceId)
                .GreaterThan(0)
                .WithMessage("Service ID must be greater than 0");

            // BookingDate validation (today or future)
            RuleFor(x => x.BookingDate)
                .Must(date => date.Date >= DateTime.UtcNow.Date)
                .WithMessage("Booking date cannot be in the past");

            // BookingDate not too far in future (3 months)
            RuleFor(x => x.BookingDate)
                .Must(date => date.Date <= DateTime.UtcNow.Date.AddMonths(3))
                .WithMessage("Booking date cannot be more than 3 months in the future");

            // BookingTime validation (30-minute intervals)
            RuleFor(x => x.BookingTime)
                .Must(time => time >= TimeSpan.Zero && time < TimeSpan.FromHours(24))
                .WithMessage("Booking time must be between 00:00 and 23:59");

            RuleFor(x => x.BookingTime)
                .Must(time => time.Minutes % 30 == 0)
                .WithMessage("Booking time must be on 30-minute intervals (e.g., 09:00, 09:30)");

            // CustomerNotes validation (optional)
            RuleFor(x => x.CustomerNotes)
                .MaximumLength(1000)
                .WithMessage("Customer notes must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.CustomerNotes));
        }
    }
}