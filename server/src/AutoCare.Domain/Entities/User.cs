using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;
using AutoCare.Domain.Enums;
using AutoCare.Domain.Events.UserEvents;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// User entity - represents all system users (Customer, Employee, Admin)
    /// Sealed to prevent inheritance - use composition instead
    /// </summary>
    public sealed class User : AuditableEntity<int>
    {
        #region Properties

        /// <summary>
        /// Gets the user's email address (lowercase, unique)
        /// </summary>
        public string Email { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the hashed password (never return in API responses)
        /// </summary>
        public string PasswordHash { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the user's full name
        /// </summary>
        public string FullName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the user's phone number (optional)
        /// </summary>
        public string? PhoneNumber { get; private set; }

        /// <summary>
        /// Gets the user type (Customer, Employee, Admin)
        /// </summary>
        public UserType UserType { get; private set; }

        /// <summary>
        /// Gets whether the user account is active
        /// </summary>
        public bool IsActive { get; private set; } = true;

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the customer profile (if user is a customer)
        /// </summary>
        public Customer? Customer { get; private set; }

        /// <summary>
        /// Gets the employee profile (if user is an employee)
        /// </summary>
        public Employee? Employee { get; private set; }

        /// <summary>
        /// Gets the collection of refresh tokens for this user
        /// </summary>
        public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private User()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new user with the specified details
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="passwordHash">Hashed password</param>
        /// <param name="fullName">User's full name</param>
        /// <param name="phoneNumber">Optional phone number</param>
        /// <param name="userType">Type of user</param>
        /// <returns>A new User entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static User Create(
            string email,
            string passwordHash,
            string fullName,
            string? phoneNumber,
            UserType userType)
        {
            // Validate inputs
            ValidateEmail(email);
            ValidatePasswordHash(passwordHash);
            ValidateFullName(fullName);

            var user = new User
            {
                Email = email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                FullName = fullName.Trim(),
                PhoneNumber = phoneNumber?.Trim(),
                UserType = userType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Raise domain event
            user.AddDomainEvent(new UserRegisteredEvent(
                user.Id,
                user.Email,
                user.FullName,
                user.UserType));

            return user;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Updates the user's profile information
        /// </summary>
        /// <param name="fullName">New full name</param>
        /// <param name="phoneNumber">New phone number</param>
        public void UpdateProfile(string fullName, string? phoneNumber)
        {
            ValidateFullName(fullName);

            FullName = fullName.Trim();
            PhoneNumber = phoneNumber?.Trim();
            MarkAsUpdated();
        }

        /// <summary>
        /// Changes the user's password
        /// </summary>
        /// <param name="newPasswordHash">New hashed password</param>
        public void ChangePassword(string newPasswordHash)
        {
            ValidatePasswordHash(newPasswordHash);

            PasswordHash = newPasswordHash;
            MarkAsUpdated();
        }

        /// <summary>
        /// Activates the user account
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                return; // Already active

            IsActive = true;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new UserActivatedEvent(Id));
        }

        /// <summary>
        /// Deactivates the user account
        /// </summary>
        /// <param name="reason">Reason for deactivation</param>
        public void Deactivate(string reason = "User requested deactivation")
        {
            if (!IsActive)
                return; // Already inactive

            IsActive = false;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new UserDeactivatedEvent(Id, reason));
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates email format and length
        /// </summary>
        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (email.Length > 255)
                throw new ArgumentException("Email cannot exceed 255 characters", nameof(email));

            // Basic email format check
            if (!email.Contains('@') || !email.Contains('.'))
                throw new ArgumentException("Invalid email format", nameof(email));
        }

        /// <summary>
        /// Validates password hash is not empty
        /// </summary>
        private static void ValidatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

            if (passwordHash.Length < 20) // BCrypt hash is typically 60+ characters
                throw new ArgumentException("Invalid password hash", nameof(passwordHash));
        }

        /// <summary>
        /// Validates full name
        /// </summary>
        private static void ValidateFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty", nameof(fullName));

            if (fullName.Length < 2)
                throw new ArgumentException("Full name must be at least 2 characters", nameof(fullName));

            if (fullName.Length > 200)
                throw new ArgumentException("Full name cannot exceed 200 characters", nameof(fullName));
        }

        #endregion

    }
}