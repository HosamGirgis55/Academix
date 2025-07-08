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

            RuleFor(x => x.NationalityId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("NationalityIsRequired"));

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

            // Experience validations
            RuleForEach(x => x.Experiences)
                .ChildRules(platform =>
                {
                    platform.RuleFor(p => p.Id)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("ExperiencePlatformIdRequired"));

                    platform.RuleFor(p => p.ProfileUrl)
                        .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
                        .WithMessage(_localizationService.GetLocalizedString("InvalidExperienceProfileUrl"));
                })
                .When(x => x.Experiences != null && x.Experiences.Any());

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

    public class CreateCertificateDtoValidator : AbstractValidator<CreateCertificateDto>
    {
        public CreateCertificateDtoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizationService.GetLocalizedString("CertificateNameRequired"))
                .MaximumLength(200).WithMessage(localizationService.GetLocalizedString("CertificateNameMaxLength"));

            RuleFor(x => x.CertificateUrl)
                .NotEmpty().WithMessage(localizationService.GetLocalizedString("CertificateUrlRequired"))
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage(localizationService.GetLocalizedString("InvalidCertificateUrl"));

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage(localizationService.GetLocalizedString("CertificateDescriptionMaxLength"));

            RuleFor(x => x.IssuedBy)
                .NotEmpty().WithMessage(localizationService.GetLocalizedString("CertificateIssuedByRequired"))
                .MaximumLength(200).WithMessage(localizationService.GetLocalizedString("CertificateIssuedByMaxLength"));

            RuleFor(x => x.IssuedDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage(localizationService.GetLocalizedString("CertificateIssuedDateInvalid"));
        }
    }

    public class CreateEducationDtoValidator : AbstractValidator<CreateEducationDto>
    {
        public CreateEducationDtoValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Degree)
                .NotEmpty().WithMessage(localizationService.GetLocalizedString("DegreeRequired"))
                .MaximumLength(200).WithMessage(localizationService.GetLocalizedString("DegreeMaxLength"));

            RuleFor(x => x.Institution)
                .NotEmpty().WithMessage(localizationService.GetLocalizedString("InstitutionRequired"))
                .MaximumLength(200).WithMessage(localizationService.GetLocalizedString("InstitutionMaxLength"));

            RuleFor(x => x.FieldOfStudy)
                .NotEmpty().WithMessage(localizationService.GetLocalizedString("FieldOfStudyRequired"))
                .MaximumLength(200).WithMessage(localizationService.GetLocalizedString("FieldOfStudyMaxLength"));

            RuleFor(x => x.StartDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage(localizationService.GetLocalizedString("StartDateInvalid"));

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage(localizationService.GetLocalizedString("EndDateMustBeAfterStartDate"))
                .When(x => x.EndDate != default(DateTime));
        }
    }
} 