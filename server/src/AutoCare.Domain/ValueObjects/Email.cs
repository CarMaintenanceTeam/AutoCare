using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoCare.Domain.ValueObjects
{
    /// <summary>
/// Email value object with validation
/// Ensures email is always valid and normalized
/// </summary>
    public sealed class Email : IEquatable<Email>
    {
        /// <summary>
    /// Gets the email address value (always lowercase)
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Private constructor to enforce factory pattern
    /// </summary>
    private Email(string value)
    {
        Value = value;
    }
    /// <summary>
    /// Creates a new Email value object with validation
    /// </summary>
    /// <param name="email">The email address string</param>
    /// <returns>A validated Email value object</returns>
    /// <exception cref="ArgumentException">Thrown when email is invalid</exception>
    public static Email Create(string email)
    {
        // Null or empty check
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        // Length check
        if (email.Length > 255)
            throw new ArgumentException("Email cannot exceed 255 characters", nameof(email));

        // Format validation
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        // Return normalized email (lowercase)
        return new Email(email.Trim().ToLowerInvariant());
    }
    /// <summary>
    /// Validates email format using regex
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        // Simple but effective email regex
        var regex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        return regex.IsMatch(email);
    }

    #region Equality & Comparison

    /// <summary>
    /// Determines whether two emails are equal
    /// </summary>
    public bool Equals(Email? other)
    {
        if (other is null)
            return false;

        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }
    /// <summary>
    /// Determines whether the specified object is equal to the current email
    /// </summary>
    public override bool Equals(object? obj)
    {
        return Equals(obj as Email);
    }

    /// <summary>
    /// Returns the hash code for this email
    /// </summary>
    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Returns the email address string
    /// </summary>
    public override string ToString()
    {
        return Value;
    }

    #endregion

        #region Operators

    /// <summary>
    /// Implicit conversion from Email to string
    /// Usage: string email = emailObject;
    /// </summary>
    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(Email? left, Email? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(Email? left, Email? right)
    {
        return !(left == right);
    }

    #endregion
        
    }
}