using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Common;

namespace AutoCare.Domain.Entities
{
    /// <summary>
    /// Refresh Token entity for JWT token rotation and management
    /// Allows users to obtain new access tokens without re-authentication
    /// </summary>
    public sealed class RefreshToken : BaseEntity<int>
    {
        #region Properties

        /// <summary>
        /// Gets the user ID this token belongs to
        /// </summary>
        public int UserId { get; private set; }

        /// <summary>
        /// Gets the refresh token value (hashed or encrypted in production)
        /// </summary>
        public string Token { get; private set; } = string.Empty;

        /// <summary>
        /// Gets when this token expires
        /// </summary>
        public DateTime ExpiresAt { get; private set; }

        /// <summary>
        /// Gets when this token was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Gets when this token was revoked (null if not revoked)
        /// </summary>
        public DateTime? RevokedAt { get; private set; }

        /// <summary>
        /// Gets whether this token has been used to generate a new token
        /// Implements token rotation for security
        /// </summary>
        public bool IsUsed { get; private set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets the user this token belongs to
        /// </summary>
        public User User { get; private set; } = null!;

        #endregion

        #region Constructors

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private RefreshToken()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a new refresh token
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="token">Token value</param>
        /// <param name="expirationDays">Days until expiration (default 7)</param>
        /// <returns>A new RefreshToken entity</returns>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        public static RefreshToken Create(int userId, string token, int expirationDays = 7)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be positive", nameof(userId));

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            if (token.Length < 32)
                throw new ArgumentException("Token must be at least 32 characters for security", nameof(token));

            if (expirationDays <= 0)
                throw new ArgumentException("Expiration days must be positive", nameof(expirationDays));

            if (expirationDays > 90)
                throw new ArgumentException("Expiration days cannot exceed 90 days", nameof(expirationDays));

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(expirationDays),
                IsUsed = false
            };

            return refreshToken;
        }

        #endregion

        #region Business Methods

        /// <summary>
        /// Marks this token as used (after generating a new token pair)
        /// Implements token rotation for security
        /// </summary>
        public void MarkAsUsed()
        {
            if (IsUsed)
                return; // Already marked as used

            if (!IsValid())
                throw new InvalidOperationException("Cannot mark invalid token as used");

            IsUsed = true;
        }

        /// <summary>
        /// Revokes this token (makes it invalid immediately)
        /// Used for logout or security incidents
        /// </summary>
        public void Revoke()
        {
            if (RevokedAt.HasValue)
                return; // Already revoked

            RevokedAt = DateTime.UtcNow;
            IsUsed = true; // Also mark as used when revoking
        }

        /// <summary>
        /// Checks if this token is valid and can be used
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            // Token is valid if:
            // 1. Not used
            // 2. Not revoked
            // 3. Not expired
            return !IsUsed
                && RevokedAt == null
                && ExpiresAt > DateTime.UtcNow;
        }

        /// <summary>
        /// Checks if this token is expired
        /// </summary>
        /// <returns>True if expired, false otherwise</returns>
        public bool IsExpired()
        {
            return ExpiresAt <= DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the remaining time until expiration
        /// </summary>
        /// <returns>TimeSpan until expiration (negative if expired)</returns>
        public TimeSpan GetRemainingLifetime()
        {
            return ExpiresAt - DateTime.UtcNow;
        }

        /// <summary>
        /// Gets a detailed status description for debugging
        /// </summary>
        /// <returns>Status description</returns>
        public string GetStatusDescription()
        {
            if (RevokedAt.HasValue)
                return $"Revoked on {RevokedAt.Value:yyyy-MM-dd HH:mm:ss}";

            if (IsUsed)
                return "Used (token rotation)";

            if (IsExpired())
                return $"Expired on {ExpiresAt:yyyy-MM-dd HH:mm:ss}";

            var remaining = GetRemainingLifetime();
            return $"Valid (expires in {remaining.Days}d {remaining.Hours}h {remaining.Minutes}m)";
        }

        #endregion
    }
}


