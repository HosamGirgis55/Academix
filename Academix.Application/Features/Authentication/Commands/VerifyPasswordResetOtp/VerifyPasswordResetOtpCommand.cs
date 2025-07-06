using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;

namespace Academix.Application.Features.Authentication.Commands.VerifyPasswordResetOtp;

public class VerifyPasswordResetOtpCommand : ICommand<Result<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Otp { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
} 