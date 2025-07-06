using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using MediatR;

namespace Academix.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommand : ICommand<string>, IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
} 