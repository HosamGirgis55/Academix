using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;

namespace Academix.Application.Features.Authentication.Commands.VerifyEmailOtp;

public class VerifyEmailOtpCommand : ICommand<Result<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
} 