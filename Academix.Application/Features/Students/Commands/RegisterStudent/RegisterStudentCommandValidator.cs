using Academix.Application.Common.Interfaces;
using FluentValidation;

namespace Academix.Application.Features.Students.Commands.RegisterStudent
{
    public class RegisterStudentCommandValidator : AbstractValidator<RegisterStudentCommand>
    {
        private readonly ILocalizationService _localizationService;

        public RegisterStudentCommandValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("FirstNameIsRequired"))
                .MinimumLength(2).WithMessage(_localizationService.GetLocalizedString("FirstNameMinLength"))
                .MaximumLength(50).WithMessage(_localizationService.GetLocalizedString("FirstNameMaxLength"));

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("LastNameIsRequired"))
                .MinimumLength(2).WithMessage(_localizationService.GetLocalizedString("LastNameMinLength"))
                .MaximumLength(50).WithMessage(_localizationService.GetLocalizedString("LastNameMaxLength"));

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("EmailIsRequired"))
                .EmailAddress().WithMessage(_localizationService.GetLocalizedString("InvalidEmailFormat"))
                .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("EmailMaxLength"));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("PasswordIsRequired"))
                .MinimumLength(6).WithMessage(_localizationService.GetLocalizedString("PasswordMinLength"))
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$")
                .WithMessage(_localizationService.GetLocalizedString("PasswordComplexity"));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("ConfirmPasswordIsRequired"))
                .Equal(x => x.Password).WithMessage(_localizationService.GetLocalizedString("PasswordsDoNotMatch"));

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(_localizationService.GetLocalizedString("InvalidGender"));

             
 
           

           
        }
    }

 



} 