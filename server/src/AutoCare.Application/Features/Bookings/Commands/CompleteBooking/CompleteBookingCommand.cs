using System;
using System.Threading;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AutoCare.Application.Features.Bookings.Commands.CompleteBooking
{
    /// <summary>
    /// Command to complete a booking (InProgress -> Completed)
    /// </summary>
    /// <param name="BookingId">Booking ID</param>
    /// <param name="StaffNotes">Optional staff notes</param>
    public sealed record CompleteBookingCommand(
        int BookingId,
        string? StaffNotes
    ) : ICommand;

    public sealed class CompleteBookingCommandHandler : ICommandHandler<CompleteBookingCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CompleteBookingCommandHandler(
            IApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        }

        public async Task Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
        {
          var userId = _currentUserService.UserId
              ?? throw new UnauthorizedException("You must be logged in to complete a booking");

          if (_currentUserService.UserType is not ("Employee" or "Admin"))
          {
              throw new ForbiddenException("Only employees or admins can complete bookings");
          }

          var booking = await _context.Bookings
              .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

          if (booking == null)
          {
              throw new NotFoundException("Booking", request.BookingId);
          }

          booking.Complete(userId);

          if (!string.IsNullOrWhiteSpace(request.StaffNotes))
          {
              booking.UpdateStaffNotes(request.StaffNotes);
          }

          await _context.SaveChangesAsync(cancellationToken);
        }
    }
}