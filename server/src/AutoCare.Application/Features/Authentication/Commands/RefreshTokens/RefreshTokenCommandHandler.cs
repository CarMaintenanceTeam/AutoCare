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

namespace AutoCare.Application.Features.Authentication.Commands.RefreshTokens
{
    /// <summary>
    /// Handler for RefreshTokenCommand
    /// Implements Token Rotation security pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles token refresh
    /// - Security First: Implements strict validation and rotation
    /// - Fail Fast: Returns early on validation failures
    /// 
    /// Security Features:
    /// - One-time use refresh tokens
    /// - Automatic token revocation on reuse attempt
    /// - User active status validation
    /// - Token expiration check
    /// </summary>
    public sealed class RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenCommandHandler> logger) : ICommandHandler<RefreshTokenCommand, AuthenticationResponse>
    {
        private readonly IApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        private readonly ILogger<RefreshTokenCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Handles the refresh token command
        /// </summary>
        /// <param name="request">Refresh token command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>New authentication response with rotated tokens</returns>
        /// <exception cref="UnauthorizedException">Thrown when token is invalid</exception>
        public async Task<AuthenticationResponse> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing refresh token request");

            // 1. Find and validate refresh token
            var refreshToken = await FindAndValidateRefreshToken(
                request.RefreshToken,
                cancellationToken);

            // 2. Load user
            var user = await LoadUser(refreshToken.UserId, cancellationToken);

            // 3. Validate user is active
            ValidateUserIsActive(user);

            // 4. Mark old token as used (Token Rotation)
            refreshToken.MarkAsUsed();

            // 5. Generate new tokens
            var authResponse = await GenerateAuthenticationResponse(user, cancellationToken);

            // 6. Save changes (mark old token as used)
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Refresh token rotated successfully for UserId: {UserId}",
                user.Id);

            return authResponse;
        }

        /// <summary>
        /// Finds and validates refresh token
        /// Implements comprehensive validation
        /// </summary>
        /// <param name="tokenValue">Refresh token value</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Valid refresh token entity</returns>
        /// <exception cref="UnauthorizedException">Thrown when token is invalid</exception>
        private async Task<Domain.Entities.RefreshToken> FindAndValidateRefreshToken(
            string tokenValue,
            CancellationToken cancellationToken)
        {
            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == tokenValue, cancellationToken);

            // Token not found
            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh token not found");
                throw UnauthorizedException.InvalidToken();
            }

            // Token already used (possible replay attack)
            if (refreshToken.IsUsed)
            {
                _logger.LogWarning(
                    "Refresh token reuse detected for UserId: {UserId}. Possible security breach!",
                    refreshToken.UserId);

                // Revoke all tokens for this user as security measure
                await RevokeAllUserTokens(refreshToken.UserId, cancellationToken);

                throw new UnauthorizedException(
                    "This refresh token has already been used. All your tokens have been revoked for security. Please login again.");
            }

            // Token revoked
            if (refreshToken.RevokedAt != null)
            {
                _logger.LogWarning(
                    "Attempt to use revoked refresh token for UserId: {UserId}",
                    refreshToken.UserId);

                throw new UnauthorizedException("This refresh token has been revoked");
            }

            // Token expired
            if (refreshToken.IsExpired())
            {
                _logger.LogWarning(
                    "Refresh token expired for UserId: {UserId}",
                    refreshToken.UserId);

                throw UnauthorizedException.TokenExpired();
            }

            return refreshToken;
        }

        /// <summary>
        /// Loads user entity
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>User entity</returns>
        /// <exception cref="NotFoundException">Thrown when user not found</exception>
        private async Task<User> LoadUser(int userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                _logger.LogError("User not found for UserId: {UserId}", userId);
                throw new NotFoundException("User", userId);
            }

            return user;
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
                    "Refresh token attempt for inactive user. UserId: {UserId}",
                    user.Id);

                throw new UnauthorizedException(
                    "Your account has been deactivated. Please contact support.");
            }
        }

        /// <summary>
        /// Revokes all refresh tokens for a user
        /// Security measure when token reuse is detected
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="cancellationToken">Cancellation token</param>
        private async Task RevokeAllUserTokens(int userId, CancellationToken cancellationToken)
        {
            var userTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in userTokens)
            {
                token.Revoke();
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogWarning(
                "Revoked {Count} refresh tokens for UserId: {UserId} due to security breach detection",
                userTokens.Count,
                userId);
        }

        /// <summary>
        /// Generates new authentication response with rotated tokens
        /// </summary>
        /// <param name="user">User entity</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with new tokens</returns>
        private async Task<AuthenticationResponse> GenerateAuthenticationResponse(
            User user,
            CancellationToken cancellationToken)
        {
            // Generate new access token
            var accessToken = _jwtTokenService.GenerateAccessToken(user);

            // Generate new refresh token
            var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

            // Create new refresh token entity
            var newRefreshToken = Domain.Entities.RefreshToken.Create(
                userId: user.Id,
                token: newRefreshTokenValue,
                expirationDays: 7);

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Build response
            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenValue,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = newRefreshToken.ExpiresAt,
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