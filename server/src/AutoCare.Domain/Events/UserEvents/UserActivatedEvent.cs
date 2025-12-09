using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.UserEvents
{
    /// <summary>
    /// Domain event raised when a user account is activated
    /// Triggers: Activation email notification
    /// </summary>
    public sealed record UserActivatedEvent : IDomainEvent
    {
        // <summary>
        /// Gets the unique identifier for this event
        /// </summary>
        public Guid EventId { get; } = Guid.NewGuid();
        /// <summary>
        /// Gets when this event occurred
        /// </summary>
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the user ID
        /// </summary>
        public int UserId { get; }

        /// <summary>
        /// Creates a new UserActivatedEvent
        /// </summary>
        public UserActivatedEvent(int userId)
        {
            UserId = userId;
        }

    }
}