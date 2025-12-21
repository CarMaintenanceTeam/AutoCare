using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Application.Features.Authentication.Models
{
    /// <summary>
    /// Data Transfer Object for authentication response
    /// Contains JWT tokens and user information
    /// </summary>
    public sealed class AuthenticationResponse
    {
        /// <summary>
        /// Gets or sets the access token (JWT)
        /// Short-lived token for API authentication
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token
        /// Long-lived token for obtaining new access tokens
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the access token expiration time (UTC)
        /// </summary>
        public DateTime AccessTokenExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the refresh token expiration time (UTC)
        /// </summary>
        public DateTime RefreshTokenExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the user information
        /// </summary>
        public UserDto User { get; set; } = null!;
    }
}