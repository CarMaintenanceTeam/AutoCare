using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Application.Features.Authentication.Models;

namespace AutoCare.Application.Features.Authentication.Commands.Login
{
    public sealed record LoginCommand(
        string Email,
        string Password
        ) : ICommand<AuthenticationResponse>;

}