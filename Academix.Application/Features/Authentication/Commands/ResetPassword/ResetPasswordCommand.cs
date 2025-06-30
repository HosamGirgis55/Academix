using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommand : ICommand<bool>, IRequest<Result<bool>>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
} 