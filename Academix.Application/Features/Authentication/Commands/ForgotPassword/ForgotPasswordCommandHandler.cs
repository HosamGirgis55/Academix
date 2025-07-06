using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Academix.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _localizationService = localizationService;
    }

    public async Task<Result<string>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Return success even if user doesn't exist for security
            return Result<string>.Success("", _localizationService.GetLocalizedString("ResetPasswordEmailSent"));
        }

        // Generate OTP for password reset
        var otp = await _emailService.GenerateOtpAsync(request.Email, "password-reset");
        
        // Update user with OTP information
        user.PasswordResetOtp = otp;
        user.PasswordResetOtpExpiry = DateTime.UtcNow.AddMinutes(15); // OTP expires in 15 minutes
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        // Send OTP via email
        var emailSent = await _emailService.SendPasswordResetOtpAsync(request.Email, otp);
        
        if (!emailSent)
        {
            return Result<string>.Failure(_localizationService.GetLocalizedString("EmailSendFailed"));
        }
        
        return Result<string>.Success("", _localizationService.GetLocalizedString("ResetPasswordEmailSent"));
    }
} 