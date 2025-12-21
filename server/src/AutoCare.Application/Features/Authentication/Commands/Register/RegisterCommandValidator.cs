using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Authentication.Commands.Register
{

    /// <summary>
    /// Validator for RegisterCommand
    /// Implements validation rules for user registration
    /// 
    /// Design Principles:
    /// - Single Responsibility: Only validates registration input
    /// - Separation of Concerns: Validation separated from business logic
    /// - Explicit Rules: Clear and testable validation rules
    /// 
    /// Validation Rules:
    /// - Email: Required, valid format, max 255 chars
    /// - Password: Required, min 8 chars, complexity requirements
    /// - FullName: Required, min 2 chars, max 200 chars
    /// - PhoneNumber: Optional, min 10 digits when provided
    /// - Address: Optional, max 500 chars
    /// - City: Optional, max 100 chars
    /// </summary>
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            // Email validation
            RuleFor(user => user.Email)
                .NotEmpty()
                    .WithMessage("Email is required")
                .EmailAddress()
                    .WithMessage("Email must be a valid email address")
                .MaximumLength(255)
                    .WithMessage("Email must not exceed 255 characters");

            // Password validation
            RuleFor(user => user.Password)
                .NotEmpty()
                    .WithMessage("Password is required")
                .MinimumLength(8)
                    .WithMessage("Password must be at least 8 characters long")
                .Matches(@"[A-Z]")
                    .WithMessage("Password must contain at least one uppercase letter")
                .Matches(@"[a-z]")
                    .WithMessage("Password must contain at least one lowercase letter")
                .Matches(@"[0-9]")
                    .WithMessage("Password must contain at least one number")
                .Matches(@"[@$!%*?&#]")
                    .WithMessage("Password must contain at least one special character (@$!%*?&#)");

            // FullName validation
            RuleFor(user => user.FullName)
                .NotEmpty()
                    .WithMessage("Full name is required")
                .MinimumLength(2)
                    .WithMessage("Full name must be at least 2 characters long")
                .MaximumLength(200)
                    .WithMessage("Full name must not exceed 200 characters")
                .Matches(@"^[a-zA-Z\s\u0600-\u06FF]+$")
                    .WithMessage("Full name must contain only letters and spaces");

            // PhoneNumber validation (optional)
            RuleFor(user => user.PhoneNumber)
                .Must(phone => string.IsNullOrWhiteSpace(phone) ||
                              phone.Replace(" ", "").Replace("-", "").Length >= 10)
                    .WithMessage("Phone number must be at least 10 digits")
                .When(user => !string.IsNullOrWhiteSpace(user.PhoneNumber));
            // Address validation (optional)
            RuleFor(user => user.Address)
                .MaximumLength(500)
                    .WithMessage("Address must not exceed 500 characters")
                .When(user => !string.IsNullOrWhiteSpace(user.Address));

            // City validation (optional)
            RuleFor(user => user.City)
                .MaximumLength(100)
                    .WithMessage("City must not exceed 100 characters")
                .When(user => !string.IsNullOrWhiteSpace(user.City));
        }
    }
}