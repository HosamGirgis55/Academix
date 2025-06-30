using Academix.Application.Common.Interfaces;
using FluentValidation;

namespace Academix.Application.Features.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(ILocalizationService localizationService)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(localizationService.GetLocalizedString("EmailRequired"))
            .EmailAddress()
            .WithMessage(localizationService.GetLocalizedString("InvalidEmailFormat"));

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(localizationService.GetLocalizedString("PasswordRequired"));
    }
} 