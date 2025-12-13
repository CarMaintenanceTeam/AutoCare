using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Interfaces
{
    /// <summary>
    /// Service for getting current date/time
    /// Allows testing with fixed dates
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Gets current local date and time
        /// </summary>
        DateTime Now { get; }

        /// <summary>
        /// Gets current UTC date and time
        /// </summary>
        DateTime UtcNow { get; }

    }
}