using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Authentication.Models;

namespace AutoCare.Application.Features.Authentication.Commands.RefreshToken
{
    /// <summary>
    /// Command to refresh access token using refresh token
    /// Implements Token Rotation security pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles token refresh
    /// - Security: Implements token rotation (one-time use)
    /// - Immutability: Record type
    /// 
    /// Token Rotation Flow:
    /// 1. Validate refresh token exists and is valid
    /// 2. Mark old refresh token as used
    /// 3. Generate new access token
    /// 4. Generate new refresh token
    /// 5. Return new tokens
    /// 
    /// Security Benefits:
    /// - Prevents token replay attacks
    /// - Limits exposure window
    /// - Detects token theft
    /// </summary>
    /// <param name="RefreshToken">Current refresh token</param>
    public sealed record RefreshTokenCommand(
        string RefreshToken
    ) : ICommand<AuthenticationResponse>;
}