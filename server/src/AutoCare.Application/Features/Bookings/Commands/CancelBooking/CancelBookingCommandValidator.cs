using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Bookings.Commands.CancelBooking
{
    /// <summary>
    /// Validator for CancelBookingCommand
    /// </summary>
    public sealed class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
    {
        public CancelBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .GreaterThan(0)
                .WithMessage("Booking ID must be greater than 0");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Cancellation reason is required")
                .MinimumLength(5)
                .WithMessage("Cancellation reason must be at least 5 characters")
                .MaximumLength(500)
                .WithMessage("Cancellation reason must not exceed 500 characters");
        }
    }
}