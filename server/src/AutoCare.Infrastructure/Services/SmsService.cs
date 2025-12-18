using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Infrastructure.Services
{
    /// <summary>
    /// Basic implementation of ISmsService
    /// TODO: Integrate with real SMS provider (Twilio, etc.)
    /// </summary>
    public class SmsService : ISmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly IApplicationDbContext _context;

        public SmsService(
            ILogger<SmsService> logger,
            IApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Sends an SMS message
        /// </summary>
        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            // TODO: Implement real SMS sending (Twilio, etc.)

            _logger.LogInformation(
                "SMS sent to {PhoneNumber}: {Message}",
                phoneNumber, message);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Sends booking reminder SMS
        /// </summary>
        public async Task SendBookingReminderAsync(int bookingId)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Customer)
                        .ThenInclude(c => c.User)
                    .Include(b => b.ServiceCenter)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                    return;

                var phoneNumber = booking.Customer.User.PhoneNumber;
                if (string.IsNullOrEmpty(phoneNumber))
                    return;

                var message = $"Reminder: You have a booking tomorrow at {booking.ServiceCenter.NameEn}. " +
                             $"Booking: {booking.BookingNumber}";

                await SendSmsAsync(phoneNumber, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending booking reminder SMS for booking {BookingId}", bookingId);
            }
        }
    }
}