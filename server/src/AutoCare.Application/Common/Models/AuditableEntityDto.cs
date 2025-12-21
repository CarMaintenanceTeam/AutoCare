using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Combined base DTO for entities with ID and auditing
    /// </summary>
    /// <typeparam name="TKey">Type of entity identifier</typeparam>
    public abstract class AuditableEntityDto<TKey> : EntityDto<TKey>
    {
        // <summary>
        /// Gets or sets when the entity was created (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the entity was last updated (UTC)
        /// Null if never updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}