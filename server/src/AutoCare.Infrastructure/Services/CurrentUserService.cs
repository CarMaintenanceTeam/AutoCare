using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;





namespace AutoCare.Infrastructure.Services
{

    /// <summary>
    /// Implementation of ICurrentUserService
    /// Extracts user information from JWT token in HTTP context
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;


        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Gets the current user ID from JWT claims
        /// </summary>
        public int? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


                return int.TryParse(userIdClaim, out var userId) ? userId : null;
            }
        }

        /// <summary>
        /// Gets the current user email from JWT claims
        /// </summary>
        public string? Email => _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

        /// <summary>
        /// Gets the current user type (role) from JWT claims
        /// </summary>
        public string? UserType => _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;

        /// <summary>
        /// Checks if user is authenticated
        /// </summary>
        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        /// <summary>
        /// Checks if user is in specified role
        /// </summary>
        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }
    }
}