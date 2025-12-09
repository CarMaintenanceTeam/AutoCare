using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Domain.ValueObjects
{
    /// <summary>
    /// Phone number value object with validation
    /// Stores only digits (no formatting characters)
    /// </summary>
    public sealed class PhoneNumber : IEquatable<PhoneNumber>
    {
        /// <summary>
        /// Gets the phone number value (digits only)
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Private constructor to enforce factory pattern
        /// </summary>
        private PhoneNumber(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new PhoneNumber value object with validation
        /// </summary>
        /// <param name="phoneNumber">The phone number string (can include formatting)</param>
        /// <returns>A validated PhoneNumber value object</returns>
        /// <exception cref="ArgumentException">Thrown when phone number is invalid</exception>
        public static PhoneNumber Create(string phoneNumber)
        {
            // Null or empty check
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty", nameof(phoneNumber));

            // Clean the phone number (remove all non-digits)
            var cleaned = CleanPhoneNumber(phoneNumber);

            // Length validation (minimum 10 digits)
            if (cleaned.Length < 10)
                throw new ArgumentException(
                    "Phone number must be at least 10 digits",
                    nameof(phoneNumber));

            // Maximum length (international format)
            if (cleaned.Length > 15)
                throw new ArgumentException(
                    "Phone number cannot exceed 15 digits",
                    nameof(phoneNumber));

            return new PhoneNumber(cleaned);
        }

        /// <summary>
        /// Removes all non-digit characters from phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number with possible formatting</param>
        /// <returns>Digits only</returns>
        private static string CleanPhoneNumber(string phoneNumber)
        {
            // Keep only digits
            return new string(phoneNumber.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Formats phone number for display (example: Egyptian format)
        /// </summary>
        /// <returns>Formatted phone number</returns>
        public string ToFormattedString()
        {
            // Example Egyptian format: 0101 234 5678
            if (Value.Length == 11 && Value.StartsWith("01"))
            {
                return $"{Value.Substring(0, 4)} {Value.Substring(4, 3)} {Value.Substring(7)}";
            }

            // Default: just return the value
            return Value;
        }

        #region Equality & Comparison

        /// <summary>
        /// Determines whether two phone numbers are equal
        /// </summary>
        public bool Equals(PhoneNumber? other)
        {
            if (other is null)
                return false;

            return Value == other.Value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current phone number
        /// </summary>
        public override bool Equals(object? obj)
        {
            return Equals(obj as PhoneNumber);
        }

        /// <summary>
        /// Returns the hash code for this phone number
        /// </summary>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Returns the phone number string (digits only)
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implicit conversion from PhoneNumber to string
        /// </summary>
        public static implicit operator string(PhoneNumber phoneNumber)
        {
            return phoneNumber.Value;
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(PhoneNumber? left, PhoneNumber? right)
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
        public static bool operator !=(PhoneNumber? left, PhoneNumber? right)
        {
            return !(left == right);
        }

        #endregion

    }
}