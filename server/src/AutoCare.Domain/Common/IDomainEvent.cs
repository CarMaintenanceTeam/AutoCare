using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AutoCare.Domain.Common
{
    /// <summary>
/// Marker interface for domain events
/// Domain events represent something that happened in the domain that domain experts care about
/// </summary>
    public interface IDomainEvent : INotification 
    {
        /// <summary>
    /// Gets the unique identifier for this event
    /// </summary>
    Guid EventId { get; }
    
    /// <summary>
    /// Gets when this event occurred
    /// </summary>
    DateTime OccurredOn { get; }
    }
}