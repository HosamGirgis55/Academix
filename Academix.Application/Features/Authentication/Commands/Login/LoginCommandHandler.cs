using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Academix.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly ILocalizationService _localizationService;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettings,
        ILocalizationService localizationService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings;
        _localizationService = localizationService;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result<AuthenticationResult>.Failure(_localizationService.GetLocalizedString("InvalidCredentials"));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Result<AuthenticationResult>.Failure(_localizationService.GetLocalizedString("InvalidCredentials"));
        }

        var authResult = await _userManager.GenerateTokenAsync(user, _jwtSettings);
        
        return Result<AuthenticationResult>.Success(authResult, _localizationService.GetLocalizedString("LoginSuccessful"));
    }
} 