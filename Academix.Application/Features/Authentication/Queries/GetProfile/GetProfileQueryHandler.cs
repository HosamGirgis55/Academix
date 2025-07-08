using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Authentication.DTOs;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Authentication.Queries.GetProfile;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<UserProfileDto>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILocalizationService _localizationService;

    public GetProfileQueryHandler(
        UserManager<ApplicationUser> userManager,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _localizationService = localizationService;
    }

    public async Task<Result<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .Include(u => u.Country)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return Result<UserProfileDto>.Failure(_localizationService.GetLocalizedString("UserNotFound"));
        }

        var roles = await _userManager.GetRolesAsync(user);

        // Get localized country name
        var countryName = user.Country.GetLocalizedName(_localizationService);

        var userProfile = new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Gender = user.Gender.GetLocalizedName(_localizationService),
            CountryId = user.CountryId,
            CountryName = countryName,
            TimeZone = user.TimeZone,
            Roles = roles.ToList()
        };

        return Result<UserProfileDto>.Success(userProfile, _localizationService.GetLocalizedString("ProfileRetrievedSuccessfully"));
    }
} 