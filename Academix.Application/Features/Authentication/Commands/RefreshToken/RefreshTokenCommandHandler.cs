using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Academix.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly ILocalizationService _localizationService;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IOptions<JwtSettings> jwtSettings,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings;
        _localizationService = localizationService;
    }

    public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var authResult = await _userManager.RefreshTokenAsync(request.RefreshToken, _jwtSettings);
        
        if (authResult == null)
        {
            return Result<AuthenticationResult>.Failure(_localizationService.GetLocalizedString("InvalidRefreshToken"));
        }

        return Result<AuthenticationResult>.Success(authResult, _localizationService.GetLocalizedString("TokenRefreshed"));
    }
} 