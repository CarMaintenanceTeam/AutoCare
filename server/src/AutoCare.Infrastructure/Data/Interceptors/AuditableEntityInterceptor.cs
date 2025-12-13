using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Domain.Common;
using AutoCare.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AutoCare.Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Interceptor that automatically sets audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
    /// for entities implementing IAuditable
    /// </summary>
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService? _currentUserService;
        private readonly IDateTime? _dateTimeService;

        /// <summary>
        /// Constructor with dependency injection
        /// </summary>
        public AuditableEntityInterceptor(
            ICurrentUserService? currentUserService = null,
            IDateTime? dateTimeService = null)
        {
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        /// <summary>
        /// Intercepts before SaveChanges (synchronous)
        /// </summary>
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            UpdateAuditableEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        /// <summary>
        /// Intercepts before SaveChangesAsync (asynchronous)
        /// </summary>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            UpdateAuditableEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        /// <summary>
        /// Updates audit fields for all tracked entities
        /// </summary>
        private void UpdateAuditableEntities(DbContext? context)
        {
            if (context == null)
                return;

            var currentTime = _dateTimeService?.UtcNow ?? DateTime.UtcNow;
            var currentUserId = _currentUserService?.UserId;

            // Get all entries that implement IAuditable
            var entries = context.ChangeTracker
                .Entries<IAuditable>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // Set creation audit fields
                    entry.Entity.CreatedAt = currentTime;
                    entry.Entity.CreatedBy = currentUserId;
                }

                if (entry.State == EntityState.Modified)
                {
                    // Set modification audit fields
                    entry.Entity.UpdatedAt = currentTime;
                    entry.Entity.UpdatedBy = currentUserId;

                    // Prevent modification of creation audit fields
                    entry.Property(nameof(IAuditable.CreatedAt)).IsModified = false;
                    entry.Property(nameof(IAuditable.CreatedBy)).IsModified = false;
                }
            }
        }
    }
}