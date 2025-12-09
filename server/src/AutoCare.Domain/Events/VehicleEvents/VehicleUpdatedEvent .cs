using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.VehicleEvents
{
    /// <summary>
    /// Domain event raised when vehicle information is updated
    /// Triggers: Update notification (optional)
    /// </summary>
    public sealed record VehicleUpdatedEvent : IDomainEvent
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
        /// Gets the vehicle ID
        /// </summary>
        public int VehicleId { get; }

        /// <summary>
        /// Gets the customer ID
        /// </summary>
        public int CustomerId { get; }

        /// <summary>
        /// Creates a new VehicleUpdatedEvent
        /// </summary>
        public VehicleUpdatedEvent(int vehicleId, int customerId)
        {
            VehicleId = vehicleId;
            CustomerId = customerId;
        }

    }
}