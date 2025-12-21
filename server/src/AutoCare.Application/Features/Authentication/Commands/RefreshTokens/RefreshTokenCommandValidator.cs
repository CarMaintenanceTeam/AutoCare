using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace AutoCare.Application.Features.Authentication.Commands.RefreshTokens
{
    /// <summary>
    /// Validator for RefreshTokenCommand
    /// Simple validation for refresh token input
    /// </summary>
    public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                    .WithMessage("Refresh token is required")
                .MinimumLength(32)
                    .WithMessage("Invalid refresh token format");
        }
    }
}