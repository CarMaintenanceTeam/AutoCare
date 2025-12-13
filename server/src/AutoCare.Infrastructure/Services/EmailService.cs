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
    /// Basic implementation of IEmailService
    /// TODO: Integrate with real email provider (SendGrid, SMTP, etc.)
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IApplicationDbContext _context;

        public EmailService(
            ILogger<EmailService> logger,
            IApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Sends a basic email
        /// </summary>
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            // TODO: Implement real email sending (SendGrid, SMTP, etc.)

            // For now, just log
            _logger.LogInformation(
                "Email sent to {To} with subject: {Subject}",
                to, subject);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Sends booking confirmation email
        /// </summary>
        public async Task SendBookingConfirmationAsync(int bookingId)
        {
            try
            {
                // Get booking details from database
                var booking = await _context.Bookings
                    .Include(b => b.Customer)
                        .ThenInclude(c => c.User)
                    .Include(b => b.Service)
                    .Include(b => b.ServiceCenter)
                    .Include(b => b.Vehicle)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                {
                    _logger.LogWarning("Booking {BookingId} not found", bookingId);
                    return;
                }

                var customerEmail = booking.Customer.User.Email;
                var subject = $"Booking Confirmation - {booking.BookingNumber}";
                var body = $@"
Dear {booking.Customer.User.FullName},

Your booking has been confirmed!

Booking Details:
- Booking Number: {booking.BookingNumber}
- Service: {booking.Service.NameEn}
- Service Center: {booking.ServiceCenter.NameEn}
- Vehicle: {booking.Vehicle.Brand} {booking.Vehicle.Model} ({booking.Vehicle.PlateNumber})
- Date: {booking.BookingDate:yyyy-MM-dd}
- Time: {booking.BookingTime:hh\\:mm}

Thank you for choosing our service!

Best regards,
Car Maintenance Platform Team
";

                await SendEmailAsync(customerEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending booking confirmation email for booking {BookingId}", bookingId);
            }
        }

        /// <summary>
        /// Sends booking cancellation email
        /// </summary>
        public async Task SendBookingCancellationAsync(int bookingId, string reason)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Customer)
                        .ThenInclude(c => c.User)
                    .Include(b => b.Service)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                    return;

                var customerEmail = booking.Customer.User.Email;
                var subject = $"Booking Cancelled - {booking.BookingNumber}";
                var body = $@"
Dear {booking.Customer.User.FullName},

Your booking has been cancelled.

Booking Number: {booking.BookingNumber}
Reason: {reason}

If you have any questions, please contact us.

Best regards,
Car Maintenance Platform Team
";

                await SendEmailAsync(customerEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending cancellation email for booking {BookingId}", bookingId);
            }
        }

        /// <summary>
        /// Sends booking status change notification
        /// </summary>
        public async Task SendBookingStatusChangeAsync(int bookingId, string oldStatus, string newStatus)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Customer)
                        .ThenInclude(c => c.User)
                    .FirstOrDefaultAsync(b => b.Id == bookingId);

                if (booking == null)
                    return;

                var customerEmail = booking.Customer.User.Email;
                var subject = $"Booking Status Update - {booking.BookingNumber}";
                var body = $@"
Dear {booking.Customer.User.FullName},

Your booking status has been updated.

Booking Number: {booking.BookingNumber}
Previous Status: {oldStatus}
New Status: {newStatus}

Best regards,
Car Maintenance Platform Team
";

                await SendEmailAsync(customerEmail, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending status change email for booking {BookingId}", bookingId);
            }
        }
    }
}