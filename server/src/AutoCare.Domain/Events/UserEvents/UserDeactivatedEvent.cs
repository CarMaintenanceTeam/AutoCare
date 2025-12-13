using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.UserEvents
{
    /// <summary>
    /// Domain event raised when a user account is deactivated
    /// Triggers: Deactivation email, Revoke all active sessions
    /// </summary>
    public sealed record UserDeactivatedEvent : IDomainEvent
    {

        /// <summary>
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
        /// Gets the reason for deactivation
        /// </summary>
        public string Reason { get; }

        /// <summary>
        /// Creates a new UserDeactivatedEvent
        /// </summary>
        public UserDeactivatedEvent(int userId, string reason)
        {
            UserId = userId;
            Reason = reason;
        }

    }
}