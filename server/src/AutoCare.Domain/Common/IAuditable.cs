using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Domain.Common
{
    /// <summary>
/// Interface for entities that require audit tracking
/// Tracks who created/updated the entity and when
/// </summary>
    public interface IAuditable
    {
        /// <summary>
    /// Gets or sets when the entity was created
    /// </summary>
    DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets when the entity was last updated
    /// </summary>
    DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the user who created this entity
    /// </summary>
    int? CreatedBy { get; set; }
    
    /// <summary>
    /// Gets or sets the ID of the user who last updated this entity
    /// </summary>
    int? UpdatedBy { get; set; }
    }
}