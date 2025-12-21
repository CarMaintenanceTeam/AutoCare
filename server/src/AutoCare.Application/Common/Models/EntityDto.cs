using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Common.Models
{
    /// <summary>
    /// Base Data Transfer Object for entities with ID
    /// Generic base class for DTOs representing entities
    /// </summary>
    /// <typeparam name="TKey">Type of entity identifier</typeparam>
    public abstract class EntityDto<TKey>
    {
        /// <summary>
        /// Gets or sets the entity identifier
        /// </summary>
        public TKey Id { get; set; } = default!;
    }
}