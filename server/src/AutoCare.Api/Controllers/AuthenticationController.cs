using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Models;
using AutoCare.Application.Features.Authentication.Commands.Login;
using AutoCare.Application.Features.Authentication.Commands.RefreshTokens;
using AutoCare.Application.Features.Authentication.Commands.Register;
using AutoCare.Application.Features.Authentication.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoCare.Api.Controllers
{
    /// <summary>
    /// Authentication controller
    /// Handles user registration, login, and token refresh
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles HTTP concerns
    /// - Dependency Inversion: Depends on IMediator abstraction
    /// - Thin Controller: Delegates all logic to handlers
    /// 
    /// Endpoints:
    /// - POST /api/auth/register - Register new customer user
    /// - POST /api/auth/login - Authenticate user
    /// - POST /api/auth/refresh-token - Refresh access token
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of AuthenticationController
    /// </remarks>
    /// <param name="mediator">MediatR mediator</param>
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public sealed class AuthenticationController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <summary>
        /// Registers a new customer user
        /// </summary>
        /// <param name="command">Registration details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with tokens</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="409">Email already exists</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(
            [FromBody] RegisterCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse<AuthenticationResponse>.SuccessResponse(result));
        }

        /// <summary>
        /// Authenticates a user and returns JWT tokens
        /// </summary>
        /// <param name="command">Login credentials</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Authentication response with tokens</returns>
        /// <response code="200">Login successful</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Invalid credentials or inactive account</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromBody] LoginCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse<AuthenticationResponse>.SuccessResponse(result));
        }

        /// <summary>
        /// Refreshes access token using refresh token
        /// Implements token rotation for security
        /// </summary>
        /// <param name="command">Refresh token</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>New authentication response with rotated tokens</returns>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="401">Invalid, expired, or revoked refresh token</response>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse<AuthenticationResponse>.SuccessResponse(result));
        }
    }
}