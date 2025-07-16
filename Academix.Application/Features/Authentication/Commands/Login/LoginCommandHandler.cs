using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
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
    private readonly ITeacherRepository _TeacherRepository;
    private readonly IStudentRepository _StudentRepository;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IOptions<JwtSettings> jwtSettings,
        ILocalizationService localizationService,
        ITeacherRepository TeacherRepository,
        IStudentRepository studentRepository)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtSettings = jwtSettings;
        _localizationService = localizationService;
        _TeacherRepository = TeacherRepository;
        _StudentRepository = studentRepository;
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

        // Update FCM token if provided
        if (!string.IsNullOrWhiteSpace(request.FcmToken))
        {
            user.DeviceToken = request.FcmToken;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result<AuthenticationResult>.Failure(_localizationService.GetLocalizedString("Failed to update device token"));
            }
        }
        var authResult = await _userManager.GenerateTokenAsync(user, _jwtSettings);
        var teacher = await _TeacherRepository.GetByEmailAsync(user.Email);
        var student = await _StudentRepository.GetStudentByUserIdAsync(user.Id);

        if (teacher != null) {
            authResult.personId = teacher.Id;

        }
        else if(student != null)
        {
            authResult.personId = student.Id;
        }
        return Result<AuthenticationResult>.Success(authResult, _localizationService.GetLocalizedString("LoginSuccessful"));
    }
} 