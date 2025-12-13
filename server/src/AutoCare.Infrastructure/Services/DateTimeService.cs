using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;

namespace AutoCare.Infrastructure.Services
{
    /// <summary>
    /// Implementation of IDateTime service
    /// Provides current date/time
    /// </summary>
    public class DateTimeService : IDateTime
    {
        /// <summary>
        /// Gets current local date and time
        /// </summary>
        public DateTime Now => DateTime.Now;

        /// <summary>
        /// Gets current UTC date and time
        /// </summary>
        public DateTime UtcNow => DateTime.UtcNow;

    }
}