using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.Enums;

namespace AutoCare.Domain.Events.UserEvents
{
    /// <summary>
    /// Domain event raised when a new user registers
    /// Triggers: Welcome email, Add to mailing list, Admin notification
    /// </summary>
    public sealed record UserRegisteredEvent : IDomainEvent
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
        /// Gets the user's email address
        /// </summary>
        public string Email { get; }

        /// <summary>
        /// Gets the user's full name
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets the type of user registered
        /// </summary>
        public UserType UserType { get; }

        /// <summary>
        /// Creates a new UserRegisteredEvent
        /// </summary>
        public UserRegisteredEvent(
            int userId,
            string email,
            string fullName,
            UserType userType)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
            UserType = userType;
        }
    }
}