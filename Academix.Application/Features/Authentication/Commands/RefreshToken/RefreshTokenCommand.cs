using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommand : ICommand<AuthenticationResult>, IRequest<Result<AuthenticationResult>>
{
    public string RefreshToken { get; set; } = string.Empty;
} 