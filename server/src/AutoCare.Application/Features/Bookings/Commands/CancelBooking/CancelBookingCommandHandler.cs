using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Bookings.Commands.CancelBooking
{
    /// <summary>
    /// Handler for CancelBookingCommand
    /// Handles cancellation by both customers and staff
    /// </summary>
    public sealed class CancelBookingCommandHandler : ICommandHandler<CancelBookingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly ILogger<CancelBookingCommandHandler> _logger;

        public CancelBookingCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IEmailService emailService,
            ILogger<CancelBookingCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to cancel a booking");

            _logger.LogInformation(
                "Cancelling booking {BookingId} by UserId: {UserId}",
                request.BookingId,
                userId);

            // Get booking with customer
            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                throw new NotFoundException("Booking", request.BookingId);
            }

            // Check authorization
            var isCustomer = booking.Customer.UserId == userId;
            var isEmployee = _currentUserService.UserType == "Employee";

            if (!isCustomer && !isEmployee)
            {
                throw new ForbiddenException("You don't have permission to cancel this booking");
            }

            // Validate cancellation is allowed
            if (isCustomer && !booking.CanBeCancelledByCustomer())
            {
                throw new BusinessRuleValidationException(
                    "This booking cannot be cancelled. It is either already in progress, completed, or cancelled.");
            }

            // Cancel booking
            booking.Cancel(request.Reason, userId);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Booking cancelled successfully. BookingId: {BookingId}, BookingNumber: {BookingNumber}",
                booking.Id,
                booking.BookingNumber);

            // Send cancellation email
            _ = Task.Run(async () =>
            {
                try
                {
                    await _emailService.SendBookingCancellationAsync(booking.Id, request.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send cancellation email");
                }
            }, cancellationToken);
        }
    }
}