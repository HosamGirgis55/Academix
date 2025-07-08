using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Authentication.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILocalizationService _localizationService;

    public UpdateProfileCommandHandler(
        UserManager<ApplicationUser> userManager,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _localizationService = localizationService;
    }

    public async Task<Result<UserProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(u => u.Country)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<UserProfileDto>.Failure(_localizationService.GetLocalizedString("UserNotFound"));
        }

        // Update user properties
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.ProfilePictureUrl = request.ProfilePictureUrl;
        user.CountryId = request.CountryId;
        
        if (Enum.TryParse<Gender>(request.Gender, true, out var gender))
        {
            user.Gender = gender;
        }
        
        // Update time zone if provided
        if (!string.IsNullOrEmpty(request.TimeZone))
        {
            user.TimeZone = request.TimeZone;
        }

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<UserProfileDto>.Failure(errors);
        }

        // Get updated user with country information
        var updatedUser = await _userManager.Users
            .Include(u => u.Country)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        var roles = await _userManager.GetRolesAsync(updatedUser!);

        // Get localized country name
        var countryName = updatedUser.Country.GetLocalizedName(_localizationService);

        var userProfile = new UserProfileDto
        {
            Id = updatedUser!.Id,
            Email = updatedUser.Email ?? string.Empty,
            FirstName = updatedUser.FirstName,
            LastName = updatedUser.LastName,
            ProfilePictureUrl = updatedUser.ProfilePictureUrl,
            Gender = updatedUser.Gender.GetLocalizedName(_localizationService),
            
            CountryName = countryName,
            TimeZone = updatedUser.TimeZone,
            Roles = roles.ToList()
        };

        return Result<UserProfileDto>.Success(userProfile, _localizationService.GetLocalizedString("ProfileUpdatedSuccessfully"));
    }
} 