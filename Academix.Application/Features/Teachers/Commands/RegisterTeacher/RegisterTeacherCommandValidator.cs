using Academix.Application.Common.Interfaces;
using FluentValidation;
using System;

namespace Academix.Application.Features.Teachers.Commands.RegisterTeacher
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

            RuleFor(x => x.Gender)
                .IsInEnum().WithMessage(_localizationService.GetLocalizedString("InvalidGender"));

            RuleFor(x => x.Bio)
                .MaximumLength(500).WithMessage(_localizationService.GetLocalizedString("BioMaxLength"));

            RuleFor(x => x.ProfilePictureUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
                .WithMessage(_localizationService.GetLocalizedString("InvalidProfilePictureUrl"));

          

            RuleFor(x => x.CountryId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("CountryIsRequired"));

            RuleFor(x => x.TeachingAreaIds)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("TeachingAreasRequired"))
                .Must(x => x.Count > 0).WithMessage(_localizationService.GetLocalizedString("TeachingAreasRequired"));

            RuleFor(x => x.AgeGroupIds)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("AgeGroupsRequired"))
                .Must(x => x.Count > 0).WithMessage(_localizationService.GetLocalizedString("AgeGroupsRequired"));

            RuleFor(x => x.CommunicationMethodIds)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("CommunicationMethodsRequired"))
                .Must(x => x.Count > 0).WithMessage(_localizationService.GetLocalizedString("CommunicationMethodsRequired"));

            RuleFor(x => x.TeachingLanguageIds)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("TeachingLanguagesRequired"))
                .Must(x => x.Count > 0).WithMessage(_localizationService.GetLocalizedString("TeachingLanguagesRequired"));

            RuleFor(x => x.Skills)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("SkillsRequired"))
                .Must(x => x.Count > 0).WithMessage(_localizationService.GetLocalizedString("SkillsRequired"));

            RuleForEach(x => x.Educations)
                .ChildRules(education =>
                {
                    education.RuleFor(e => e.Institution)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("InstitutionRequired"))
                        .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("InstitutionMaxLength"));

                    education.RuleFor(e => e.Degree)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("DegreeRequired"))
                        .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("DegreeMaxLength"));

                    education.RuleFor(e => e.Field)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("FieldRequired"))
                        .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("FieldMaxLength"));

                    education.RuleFor(e => e.StartDate)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("StartDateRequired"))
                        .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(_localizationService.GetLocalizedString("StartDateInFuture"));

                    education.RuleFor(e => e.EndDate)
                        .GreaterThanOrEqualTo(e => e.StartDate)
                        .When(e => e.EndDate.HasValue)
                        .WithMessage(_localizationService.GetLocalizedString("EndDateBeforeStartDate"));
                });

            RuleForEach(x => x.Certificates)
                .ChildRules(certificate =>
                {
                    certificate.RuleFor(c => c.Name)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("CertificateNameRequired"))
                        .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("CertificateNameMaxLength"));

                    certificate.RuleFor(c => c.CertificateUrl)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("CertificateUrlRequired"))
                        .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                        .WithMessage(_localizationService.GetLocalizedString("InvalidCertificateUrl"));

                    certificate.RuleFor(c => c.IssuedBy)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("IssuedByRequired"))
                        .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("IssuedByMaxLength"));

                    certificate.RuleFor(c => c.IssuedDate)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("IssuedDateRequired"))
                        .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(_localizationService.GetLocalizedString("IssuedDateInFuture"));
                });

            RuleForEach(x => x.Exams)
                .ChildRules(exam =>
                {
                    exam.RuleFor(e => e.Name)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("ExamNameRequired"))
                        .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("ExamNameMaxLength"));

                    exam.RuleFor(e => e.ExamResult)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("ExamResultRequired"))
                        .MaximumLength(50).WithMessage(_localizationService.GetLocalizedString("ExamResultMaxLength"));

                    exam.RuleFor(e => e.IssuedBy)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("ExamIssuedByRequired"))
                        .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("ExamIssuedByMaxLength"));

                    exam.RuleFor(e => e.IssuedDate)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("ExamIssuedDateRequired"))
                        .LessThanOrEqualTo(DateTime.UtcNow).WithMessage(_localizationService.GetLocalizedString("ExamIssuedDateInFuture"));

                    exam.RuleFor(e => e.ExameCertificateUrl)
                        .Must(uri => string.IsNullOrEmpty(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
                        .WithMessage(_localizationService.GetLocalizedString("InvalidExamCertificateUrl"));
                });

            RuleForEach(x => x.Skills)
                .ChildRules(skill =>
                {
                    skill.RuleFor(s => s.SkillId)
                        .NotEmpty().WithMessage(_localizationService.GetLocalizedString("SkillIdRequired"));

                   
                });
        }
    }
} 