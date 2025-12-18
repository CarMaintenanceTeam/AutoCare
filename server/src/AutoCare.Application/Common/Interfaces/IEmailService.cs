using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Service for sending emails
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email
        /// </summary>
        Task SendEmailAsync(string to, string subject, string body);

        /// <summary>
        /// Sends booking confirmation email
        /// </summary>
        Task SendBookingConfirmationAsync(int bookingId);

        /// <summary>
        /// Sends booking cancellation email
        /// </summary>
        Task SendBookingCancellationAsync(int bookingId, string reason);

        /// <summary>
        /// Sends booking status change notification
        /// </summary>
        Task SendBookingStatusChangeAsync(int bookingId, string oldStatus, string newStatus);
    }
}