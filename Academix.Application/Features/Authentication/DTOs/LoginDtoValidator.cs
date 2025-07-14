using Academix.Application.Common.Interfaces;
using FluentValidation;

namespace Academix.Application.Features.Authentication.DTOs;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(localizationService.GetLocalizedString("EmailRequired"))
            .EmailAddress()
            .WithMessage(localizationService.GetLocalizedString("InvalidEmailFormat"))
            .MaximumLength(100)
            .WithMessage(localizationService.GetLocalizedString("EmailMaxLength"));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(localizationService.GetLocalizedString("PasswordRequired"))
            .MinimumLength(6)
            .WithMessage(localizationService.GetLocalizedString("PasswordMinLength"));
    }
} 