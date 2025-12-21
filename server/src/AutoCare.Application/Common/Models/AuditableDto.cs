using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Base Data Transfer Object for entities with auditing
    /// Implements DTO pattern for data transfer across boundaries
    /// 
    /// Design Principles:
    /// - Single Responsibility: Represents auditable data
    /// - Open/Closed: Open for extension by specific DTOs
    /// - Data Transfer Object: No business logic, only data
    /// </summary>
    public abstract class AuditableDto
    {
        /// <summary>
        /// Gets or sets when the entity was created (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the entity was last updated (UTC)
        /// Null if never updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets who created the entity
        /// Null if created by system
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets who last updated the entity
        /// Null if never updated
        /// </summary>
        public int? UpdatedBy { get; set; }
    }
}