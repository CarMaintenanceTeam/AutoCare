using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Domain.Common
{
    /// <summary>
/// Base entity class that all domain entities inherit from
/// Provides:
/// - Entity identification
/// - Domain events management
/// - Equality comparison based on ID
/// </summary>
/// <typeparam name="TKey">Type of the entity identifier</typeparam>
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        // Private list to store domain events
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets or sets the unique identifier for this entity
    /// Protected set to allow derived classes to set it (for EF Core)
    /// </summary>
    public TKey Id { get; protected set; } = default!;

    /// <summary>
    /// Gets the read-only collection of domain events raised by this entity
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to be dispatched when the entity is saved
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a specific domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to remove</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events
    /// Called after events are dispatched
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    #region Equality Comparison

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity
    /// Entities are equal if they have the same type and the same ID
    /// </summary>
    public override bool Equals(object? obj)
    {
        // Check if obj is null
        if (obj is null)
            return false;

        // Check if obj is the same reference
        if (ReferenceEquals(this, obj))
            return true;

        // Check if obj is not the same type
        if (obj.GetType() != GetType())
            return false;

        // Cast to BaseEntity
        if (obj is not BaseEntity<TKey> other)
            return false;

        // Check if either entity has default ID (transient entity)
        if (EqualityComparer<TKey>.Default.Equals(Id, default) || 
            EqualityComparer<TKey>.Default.Equals(other.Id, default))
            return false;

        // Compare IDs
        return EqualityComparer<TKey>.Default.Equals(Id, other.Id);
    }

    /// <summary>
    /// Returns a hash code for the entity based on its ID
    /// </summary>
    public override int GetHashCode()
    {
        return EqualityComparer<TKey>.Default.GetHashCode(Id!) * 41 + GetType().GetHashCode();
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(BaseEntity<TKey>? a, BaseEntity<TKey>? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(BaseEntity<TKey>? a, BaseEntity<TKey>? b)
    {
        return !(a == b);
    }

    #endregion
    }
}