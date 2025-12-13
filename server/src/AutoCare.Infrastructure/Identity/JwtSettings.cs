using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoCare.Infrastructure.Identity
{
    /// <summary>
    /// JWT configuration settings
    /// Maps to appsettings.json "Jwt" section
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Section name in appsettings.json
        /// </summary>
        public const string SectionName = "Jwt";

        /// <summary>
        /// Secret key for signing tokens (minimum 32 characters)
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Token issuer (usually your application name)
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Token audience (who can use this token)
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Access token expiration in minutes (default: 15 minutes)
        /// </summary>
        public int AccessTokenExpirationMinutes { get; set; } = 15;

        /// <summary>
        /// Refresh token expiration in days (default: 7 days)
        /// </summary>
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}