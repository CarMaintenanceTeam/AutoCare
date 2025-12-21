using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Exceptions;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Authentication.Models;
using AutoCare.Domain.Entities;

using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Application.Features.Authentication.Commands.Register
{
    /// <summary>
    /// Handler for RegisterCommand
    /// Implements Command Handler pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles user registration logic
    /// - Dependency Inversion: Depends on abstractions (interfaces)
    /// - Open/Closed: Can be extended without modification
    /// 
    /// Dependencies:
    /// - IApplicationDbContext: Database access
    /// - IPasswordHasher: Password hashing
    /// - IJwtTokenService: Token generation
    /// - ILogger: Logging
    /// 
    /// Transaction Management:
    /// Uses implicit transaction from SaveChangesAsync
    /// Both User and Customer are created in same transaction
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of RegisterCommandHandler
    /// </remarks>
    public sealed class RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ILogger<RegisterCommandHandler> logger) : ICommandHandler<RegisterCommand, AuthenticationResponse>
    {
        private readonly IApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IPasswordHasher _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
        private readonly ILogger<RegisterCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Handles the registration command
        /// </summary>
        /// <param name="request">Registration command</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with tokens</returns>
        /// <exception cref="DuplicateException">Thrown when email already exists</exception>
        public async Task<AuthenticationResponse> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Processing registration request for email: {Email}",
                request.Email);

            // 1. Check if email already exists
            await ValidateEmailUniqueness(request.Email, cancellationToken);

            // 2. Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // 3. Create User entity
            var user = User.Create(
                email: request.Email,
                passwordHash: passwordHash,
                fullName: request.FullName,
                phoneNumber: request.PhoneNumber,
                userType: UserType.Customer);

            _context.Users.Add(user);

            // 4. Create Customer profile
            var customer = Customer.Create(
                userId: user.Id,
                address: request.Address,
                city: request.City);

            _context.Customers.Add(customer);

            // 5. Save to database (atomic transaction)
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "User registered successfully. UserId: {UserId}, Email: {Email}",
                user.Id,
                user.Email);

            // 6. Generate tokens
            var authResponse = await GenerateAuthenticationResponse(user, cancellationToken);

            return authResponse;
        }

        /// <summary>
        /// Validates that email is unique
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <exception cref="DuplicateException">Thrown when email exists</exception>
        private async Task ValidateEmailUniqueness(string email, CancellationToken cancellationToken)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == email.ToLower(), cancellationToken);

            if (emailExists)
            {
                _logger.LogWarning(
                    "Registration failed: Email {Email} already exists",
                    email);

                throw new DuplicateException("User", "Email", email);
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

            _logger.LogInformation(
                "Tokens generated successfully for UserId: {UserId}",
                user.Id);

            // Build response
            return new AuthenticationResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15), // From JWT settings
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