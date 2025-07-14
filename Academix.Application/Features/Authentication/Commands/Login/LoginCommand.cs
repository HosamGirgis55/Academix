using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.DTOs;
using MediatR;

namespace Academix.Application.Features.Authentication.Commands.Login;

public class LoginCommand : IRequest<Result<AuthenticationResult>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? FcmToken { get; set; }
} 