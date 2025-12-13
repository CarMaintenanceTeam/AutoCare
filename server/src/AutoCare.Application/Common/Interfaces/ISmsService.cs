using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Service for sending SMS messages
    /// </summary>
    public interface ISmsService
    {
        /// <summary>
        /// Sends an SMS message
        /// </summary>
        Task SendSmsAsync(string phoneNumber, string message);

        /// <summary>
        /// Sends booking reminder SMS
        /// </summary>
        Task SendBookingReminderAsync(int bookingId);
    }
}