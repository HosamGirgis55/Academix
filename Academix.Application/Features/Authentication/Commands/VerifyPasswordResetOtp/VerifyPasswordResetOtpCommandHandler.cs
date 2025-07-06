using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Academix.Application.Features.Authentication.Commands.VerifyPasswordResetOtp;

public class VerifyPasswordResetOtpCommandHandler : IRequestHandler<VerifyPasswordResetOtpCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public VerifyPasswordResetOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _localizationService = localizationService;
    }

    public async Task<Result<string>> Handle(VerifyPasswordResetOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            return Result<string>.Failure(_localizationService.GetLocalizedString("UserNotFound"));
        }

        // Validate OTP using the email service
        var isOtpValid = await _emailService.ValidateOtpAsync(request.Email, request.Otp, "password-reset");
        
        if (!isOtpValid)
        {
            return Result<string>.Failure(_localizationService.GetLocalizedString("InvalidOtp"));
        }

        // Reset the password
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<string>.Failure($"Password reset failed: {errors}");
        }

        // Clear OTP fields
        user.PasswordResetOtp = null;
        user.PasswordResetOtpExpiry = null;
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result<string>.Success(
            _localizationService.GetLocalizedString("PasswordResetSuccessfully"),
            _localizationService.GetLocalizedString("PasswordResetSuccessfully")
        );
    }
} 