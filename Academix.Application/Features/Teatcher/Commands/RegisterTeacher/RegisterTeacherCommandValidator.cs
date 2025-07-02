using Academix.Application.Common.Interfaces;
using FluentValidation;

namespace Academix.Application.Features.Teacher.Commands.RegisterStudent
{
    public class RegisterTeacherCommandValidator : AbstractValidator<RegisterTeacherCommand>
    {
        private readonly ILocalizationService _localizationService;

        public RegisterTeacherCommandValidator(ILocalizationService localizationService)
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

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("CountryIsRequired"));
            RuleFor(x => x.NationalityId)
               .NotEmpty().WithMessage(_localizationService.GetLocalizedString("NationalityIsRequired"));


            //// Certificate validations
            //RuleForEach(x => x.Certificates)
            //    .SetValidator(new CreateCertificateDtoValidator(_localizationService))
            //    .When(x => x.Certificates != null && x.Certificates.Any());

            //// Education validations
            //RuleForEach(x => x.Educations)
            //    .SetValidator(new CreateEducationDtoValidator(_localizationService))
            //    .When(x => x.Educations != null && x.Educations.Any());
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