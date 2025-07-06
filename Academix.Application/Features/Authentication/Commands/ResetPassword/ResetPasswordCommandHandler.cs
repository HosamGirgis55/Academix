using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Academix.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILocalizationService _localizationService;

    public ResetPasswordCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _localizationService = localizationService;
    }

    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || user.ResetPasswordToken != request.Token || 
            user.ResetPasswordTokenExpiry == null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
        {
            return Result<bool>.Failure(_localizationService.GetLocalizedString("InvalidResetToken"));
        }

        // Remove the old password
        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
        if (!removePasswordResult.Succeeded)
        {
            return Result<bool>.Failure(_localizationService.GetLocalizedString("PasswordResetFailed"));
        }

        // Add the new password
        var addPasswordResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
        if (!addPasswordResult.Succeeded)
        {
            var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
            return Result<bool>.Failure(errors);
        }

        // Clear the reset token
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result<bool>.Success(true, _localizationService.GetLocalizedString("PasswordResetSuccessful"));
    }
} 