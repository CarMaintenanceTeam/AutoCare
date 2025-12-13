using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Domain.Entities;

namespace AutoCare.Infrastructure.Identity
{
    /// <summary>
    /// Service for generating and validating JWT tokens
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generates access token (JWT)
        /// </summary>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Generates refresh token
        /// </summary>
        string GenerateRefreshToken();

        /// <summary>
        /// Validates access token and returns user ID
        /// </summary>
        int? ValidateAccessToken(string token);
    }
}