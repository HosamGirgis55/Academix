using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace Academix.Application.Features.Authentication.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<string>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILocalizationService _localizationService;

    public ForgotPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
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

        // Generate password reset token
        var resetToken = GenerateSecureToken();
        user.ResetPasswordToken = resetToken;
        user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1); // Token expires in 1 hour

        await _userManager.UpdateAsync(user);

        // TODO: Send email with reset token
        // In a real application, you would send an email here with the reset link
        // For now, we'll return the token (in production, this should not be returned)
        
        return Result<string>.Success(resetToken, _localizationService.GetLocalizedString("ResetPasswordEmailSent"));
    }

    private static string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
} 