using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Authentication.Models;

namespace AutoCare.Application.Features.Authentication.Commands.Register
{
    /// <summary>
    /// Command to register a new customer user
    /// Implements CQRS Command pattern
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only handles user registration
    /// - Immutability: Record type ensures immutability
    /// - Command Pattern: Encapsulates registration request
    /// 
    /// Flow:
    /// 1. Validate input (RegisterCommandValidator)
    /// 2. Check email uniqueness
    /// 3. Hash password
    /// 4. Create User entity
    /// 5. Create Customer profile
    /// 6. Generate JWT tokens
    /// 7. Return authentication response
    /// </summary>
    /// <param name="Email">User's email address (unique identifier)</param>
    /// <param name="Password">User's password (will be hashed)</param>
    /// <param name="FullName">User's full name</param>
    /// <param name="PhoneNumber">User's phone number (optional)</param>
    /// <param name="Address">Customer's address (optional)</param>
    /// <param name="City">Customer's city (optional)</param>
    public sealed record RegisterCommand(
        string Email,
        string Password,
        string FullName,
        string? PhoneNumber,
        string? Address,
        string? City
        ) : ICommand<AuthenticationResponse>;

}