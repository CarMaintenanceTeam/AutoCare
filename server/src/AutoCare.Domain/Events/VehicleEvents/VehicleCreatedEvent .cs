using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Events.VehicleEvents
{
    /// <summary>
    /// Domain event raised when a new vehicle is added by a customer
    /// Triggers: Vehicle registration confirmation
    /// </summary>
    public sealed record VehicleCreatedEvent : IDomainEvent
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
        /// Gets the customer ID who owns the vehicle
        /// </summary>
        public int CustomerId { get; }

        /// <summary>
        /// Gets the vehicle plate number
        /// </summary>
        public string PlateNumber { get; }

        /// <summary>
        /// Gets the vehicle brand and model
        /// </summary>
        public string VehicleInfo { get; }

        /// <summary>
        /// Creates a new VehicleCreatedEvent
        /// </summary>
        public VehicleCreatedEvent(
            int vehicleId,
            int customerId,
            string plateNumber,
            string vehicleInfo)
        {
            VehicleId = vehicleId;
            CustomerId = customerId;
            PlateNumber = plateNumber;
            VehicleInfo = vehicleInfo;
        }


    }

}