using System;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutoCare.Application.Features.Bookings.Commands.StartBooking
{
    /// <summary>
    /// Command to start work on a booking (Confirmed -> InProgress)
    /// </summary>
    /// <param name="BookingId">Booking ID</param>
    /// <param name="StaffNotes">Optional staff notes</param>
    public sealed record StartBookingCommand(
        int BookingId,
        string? StaffNotes
    ) : ICommand;

    public sealed class StartBookingCommandHandler : ICommandHandler<StartBookingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public StartBookingCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task Handle(StartBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("You must be logged in to start a booking");

            if (_currentUserService.UserType is not ("Employee" or "Admin"))
            {
                throw new ForbiddenException("Only employees or admins can start bookings");
            }

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null)
            {
                throw new NotFoundException("Booking", request.BookingId);
            }

            booking.StartProgress(userId);

            if (!string.IsNullOrWhiteSpace(request.StaffNotes))
            {
                booking.UpdateStaffNotes(request.StaffNotes);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}