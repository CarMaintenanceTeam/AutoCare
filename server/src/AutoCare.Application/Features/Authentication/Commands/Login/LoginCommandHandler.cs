using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Authentication.Models;
using AutoCare.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Authentication.Commands.Login
{
    /// <summary>
    /// Handler for LoginCommand
    /// Implements Command Handler pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles user authentication
    /// - Dependency Inversion: Depends on abstractions
    /// - Fail Fast: Returns early on validation failures
    /// 
    /// Security Considerations:
    /// - Password verification using BCrypt
    /// - Generic error message for invalid credentials (security best practice)
    /// - Active user check
    /// - Old refresh tokens cleanup
    /// </summary>
    public sealed class LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<LoginCommandHandler> logger) : ICommandHandler<LoginCommand, AuthenticationResponse>
    {
        private readonly IApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IPasswordHasher _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        private readonly ILogger<LoginCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Handles the login command
        /// </summary>
        /// <param name="request">Login command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with tokens</returns>
        /// <exception cref="UnauthorizedException">Thrown when credentials are invalid</exception>
        public async Task<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
            "Processing login request for email: {Email}",
            request.Email);

            // 1. Find user by email
            var user = await FindUserByEmail(request.Email, cancellationToken);

            // 2. Verify password
            VerifyPassword(user, request.Password);

            // 3. Check if user is active
            ValidateUserIsActive(user);

            // 4. Clean up old refresh tokens
            await CleanupOldRefreshTokens(user.Id, cancellationToken);

            // 5. Generate new tokens
            var authResponse = await GenerateAuthenticationResponse(user, cancellationToken);

            _logger.LogInformation(
                "User logged in successfully. UserId: {UserId}",
                user.Id);

            return authResponse;
        }

        /// <summary>
        /// Finds user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User entity</returns>
        /// <exception cref="UnauthorizedException">Thrown when user not found</exception>
        private async Task<User> FindUserByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLower(), cancellationToken);

            if (user == null)
            {
                _logger.LogWarning(
                    "Login failed: User with email {Email} not found",
                    email);

                // Generic error message for security (don't reveal if email exists)
                throw UnauthorizedException.InvalidCredentials();
            }

            return user;
        }

        /// <summary>
        /// Verifies password against hash
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="password">Password to verify</param>
        /// <exception cref="UnauthorizedException">Thrown when password is invalid</exception>
        private void VerifyPassword(User user, string password)
        {
            var isPasswordValid = _passwordHasher.VerifyPassword(password, user.PasswordHash);

            if (!isPasswordValid)
            {
                _logger.LogWarning(
                    "Login failed: Invalid password for UserId: {UserId}",
                    user.Id);

                // Generic error message for security
                throw UnauthorizedException.InvalidCredentials();
            }
        }

        /// <summary>
        /// Validates that user account is active
        /// </summary>
        /// <param name="user">User entity</param>
        /// <exception cref="UnauthorizedException">Thrown when user is inactive</exception>
        private void ValidateUserIsActive(User user)
        {
            if (!user.IsActive)
            {
                _logger.LogWarning(
                    "Login failed: User account is inactive. UserId: {UserId}",
                    user.Id);

                throw new UnauthorizedException(
                    "Your account has been deactivated. Please contact support.");
            }
        }

        /// <summary>
        /// Cleans up expired and used refresh tokens
        /// Implements token rotation security best practice
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task CleanupOldRefreshTokens(int userId, CancellationToken cancellationToken)
        {
            // Get expired or used tokens
            var oldTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId &&
                            (rt.ExpiresAt < DateTime.UtcNow || rt.IsUsed || rt.RevokedAt != null))
                .ToListAsync(cancellationToken);

            if (oldTokens.Any())
            {
                _context.RefreshTokens.RemoveRange(oldTokens);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Cleaned up {Count} old refresh tokens for UserId: {UserId}",
                    oldTokens.Count,
                    userId);
            }
        }

        /// <summary>
        /// Generates authentication response with tokens
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response</returns>
        private async Task<AuthenticationResponse> GenerateAuthenticationResponse(
            User user,
            CancellationToken cancellationToken)
        {
            // Generate access token
            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            // Generate refresh token
            var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            // Create refresh token entity
            var refreshToken = RefreshToken.Create(
                userId: user.Id,
                token: refreshTokenValue,
                expirationDays: 7);

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Build response
            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = refreshToken.ExpiresAt,
                User = new UserDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    UserType = user.UserType.ToString(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                }
            };
        }

    }
}