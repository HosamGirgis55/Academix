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

             

            RuleFor(x => x.ResidenceCountryId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("CountryIsRequired"));

            RuleFor(x => x.LevelId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("LevelIsRequired"));

            RuleFor(x => x.GraduationStatusId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("GraduationStatusIsRequired"));

            RuleFor(x => x.SpecialistId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("SpecialistIsRequired"));

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage(_localizationService.GetLocalizedString("BioMaxLength"));

            RuleFor(x => x.Github)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage(_localizationService.GetLocalizedString("InvalidGithubUrl"));

            RuleFor(x => x.ProfilePictureUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage(_localizationService.GetLocalizedString("InvalidProfilePictureUrl"));

            

            // Skills validations
            RuleForEach(x => x.Skills)
                .ChildRules(skill =>
                {
                    skill.RuleFor(s => s.SkillId)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("SkillIdRequired"));
 
                })
                .When(x => x.Skills != null && x.Skills.Any());
        }
    }

    

 



} 