using System;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutoCare.Application.Features.Bookings.Commands.ConfirmBooking
{
    /// <summary>
    /// Command to confirm a booking (Pending -> Confirmed)
    /// Executed by employees/admins.
    /// </summary>
    /// <param name="BookingId">Booking ID</param>
    /// <param name="StaffNotes">Optional staff notes</param>
    public sealed record ConfirmBookingCommand(
        int BookingId,
        string? StaffNotes
    ) : ICommand;

    /// <summary>
    /// Handler for ConfirmBookingCommand
    /// </summary>
    public sealed class ConfirmBookingCommandHandler : ICommandHandler<ConfirmBookingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ConfirmBookingCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to confirm a booking");

            // Only Employee/Admin can confirm
            if (_currentUserService.UserType is not ("Employee" or "Admin"))
            {
                throw new ForbiddenException("Only employees or admins can confirm bookings");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                throw new NotFoundException("Booking", request.BookingId);
            }

            booking.Confirm(userId, request.StaffNotes);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}