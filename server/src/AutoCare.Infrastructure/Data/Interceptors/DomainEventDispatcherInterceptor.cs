using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AutoCare.Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Interceptor that dispatches domain events before saving changes
    /// Ensures domain events are published as part of the same transaction
    /// </summary>
    public class DomainEventDispatcherInterceptor : SaveChangesInterceptor
    {
        private readonly IMediator? _mediator;

        /// <summary>
        /// Constructor with dependency injection
        /// </summary>
        public DomainEventDispatcherInterceptor(IMediator? mediator = null)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Intercepts before SaveChanges (synchronous)
        /// </summary>
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChanges(eventData, result);
        }

        /// <summary>
        /// Intercepts before SaveChangesAsync (asynchronous)
        /// </summary>
        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(eventData.Context);
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Dispatches all domain events from tracked entities
        /// </summary>
        private async Task DispatchDomainEventsAsync(DbContext? context)
        {
            if (context == null || _mediator == null)
                return;

            // Get all entities that have domain events
            var entitiesWithEvents = context.ChangeTracker
                .Entries<BaseEntity<int>>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            // Collect all domain events
            var domainEvents = entitiesWithEvents
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear domain events from entities
            entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

            // Dispatch all events using MediatR
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}