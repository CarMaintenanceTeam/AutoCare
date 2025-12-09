using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Domain.Common
{
    /// <summary>
/// Base interface for all entities in the domain
/// Provides a contract for entity identification
/// </summary>
/// <typeparam name="TKey">Type of the entity identifier (usually int, Guid, or long)</typeparam>
public interface IEntity<TKey>
    {
           /// <summary>
    /// Gets the unique identifier for this entity
    /// </summary>
    TKey Id { get; } 
    }
}