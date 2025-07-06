using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Authentication.Commands.ChangePassword;

public class ChangePasswordCommand : ICommand<bool>, IRequest<Result<bool>>
{
    public string UserId { get; set; } = string.Empty;
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
} 