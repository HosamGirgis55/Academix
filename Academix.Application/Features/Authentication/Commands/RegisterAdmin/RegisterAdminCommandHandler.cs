using Academix.Application.Common.Extensions;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Academix.Application.Features.Authentication.Commands.RegisterAdmin
{
    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, Result<AuthenticationResult>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILocalizationService _localizationService;
        private readonly IEmailService _emailService;
        private readonly IOptions<JwtSettings> _jwtSettings;

        public RegisterAdminCommandHandler(
            UserManager<ApplicationUser> userManager,
            ILocalizationService localizationService,
            IEmailService emailService,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _localizationService = localizationService;
            _emailService = emailService;
            _jwtSettings = jwtSettings;
        }

        public async Task<Result<AuthenticationResult>> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if user exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result<AuthenticationResult>.Failure(_localizationService.GetLocalizedString("UserAlreadyExists"));
                }

                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = (Gender)request.Gender,
                    CountryId = request.CountryId,
                    ProfilePictureUrl = request.ProfilePictureUrl,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true // Admins are auto-confirmed
                };

                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<AuthenticationResult>.Failure($"{_localizationService.GetLocalizedString("UserCreationFailed")}: {errors}");
                }

                // Add to Admin role
                await _userManager.AddToRoleAsync(user, "Admin");

                // Generate JWT tokens for automatic login
                var authResult = await _userManager.GenerateTokenAsync(user, _jwtSettings);

                return Result<AuthenticationResult>.Success(authResult, _localizationService.GetLocalizedString("AdminRegistrationSuccessful"));
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResult>.Failure($"{_localizationService.GetLocalizedString("AdminRegistrationFailed")}: {ex.Message}");
            }
        }
    }
} 