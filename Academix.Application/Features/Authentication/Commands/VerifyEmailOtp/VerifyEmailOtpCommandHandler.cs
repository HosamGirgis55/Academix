using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Academix.Application.Features.Authentication.Commands.VerifyEmailOtp;

public class VerifyEmailOtpCommandHandler : IRequestHandler<VerifyEmailOtpCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly ILocalizationService _localizationService;

    public VerifyEmailOtpCommandHandler(
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _emailService = emailService;
        _localizationService = localizationService;
    }

    public async Task<Result<string>> Handle(VerifyEmailOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            return Result<string>.Failure(_localizationService.GetLocalizedString("UserNotFound"));
        }

        if (user.EmailConfirmed)
        {
            return Result<string>.Failure(_localizationService.GetLocalizedString("EmailAlreadyConfirmed"));
        }

        // Validate OTP using the email service
        var isOtpValid = await _emailService.ValidateOtpAsync(request.Email, request.Otp, "registration");
        
        if (!isOtpValid)
        {
            return Result<string>.Failure(_localizationService.GetLocalizedString("InvalidOtp"));
        }

        // Confirm the email
        user.EmailConfirmed = true;
        user.EmailOtp = null;
        user.EmailOtpExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            return Result<string>.Failure(_localizationService.GetLocalizedString("EmailConfirmationFailed"));
        }

        return Result<string>.Success(
            _localizationService.GetLocalizedString("EmailConfirmedSuccessfully"),
            _localizationService.GetLocalizedString("EmailConfirmedSuccessfully")
        );
    }
} 