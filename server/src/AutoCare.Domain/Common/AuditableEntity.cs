using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Domain.Common
{
    /// <summary>
/// Base auditable entity that tracks creation and modification
/// Inherits from BaseEntity and implements IAuditable
/// </summary>
/// <typeparam name="TKey">Type of the entity identifier</typeparam>
        public abstract class AuditableEntity<TKey> : BaseEntity<TKey>,IAuditable
    {
        /// <summary>
    /// Gets or sets when this entity was created
    /// Default value is current UTC time
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when this entity was last updated
    /// Null if never updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets who created this entity
    /// Null if created by system or anonymous
    /// </summary>
    public int? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets who last updated this entity
    /// Null if never updated
    /// </summary>
    public int? UpdatedBy { get; set; }

    /// <summary>
    /// Marks this entity as updated with current timestamp
    /// </summary>
    /// <param name="updatedBy">Optional: ID of the user making the update</param>
    public void MarkAsUpdated(int? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
    }
}