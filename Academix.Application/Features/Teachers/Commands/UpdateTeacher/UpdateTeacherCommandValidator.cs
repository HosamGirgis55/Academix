using Academix.Application.Common.Interfaces;
using Academix.Domain.Enums;
using FluentValidation;

namespace Academix.Application.Features.Teachers.Commands.UpdateTeacher;

public class UpdateTeacherCommandValidator : AbstractValidator<UpdateTeacherCommand>
{
    public UpdateTeacherCommandValidator(ILocalizationService localizationService)
    {
        // User ID validation (required)
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(localizationService.GetLocalizedString("UserIdRequired"));

        // User Info validation - only validate if provided
        RuleFor(x => x.FirstName)
            .Length(2, 100)
            .WithMessage(localizationService.GetLocalizedString("FirstNameLength"))
            .When(x => !string.IsNullOrEmpty(x.FirstName));

        RuleFor(x => x.LastName)
            .Length(2, 100)
            .WithMessage(localizationService.GetLocalizedString("LastNameLength"))
            .When(x => !string.IsNullOrEmpty(x.LastName));

        // Gender validation - only validate if provided
        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage(localizationService.GetLocalizedString("InvalidGender"))
            .When(x => x.Gender.HasValue);

        // Teacher-specific validation - only validate if provided
        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .WithMessage(localizationService.GetLocalizedString("BioMaxLength"))
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0)
            .WithMessage(localizationService.GetLocalizedString("SalaryMustBePositive"))
            .LessThanOrEqualTo(10000)
            .WithMessage(localizationService.GetLocalizedString("SalaryMaxValue"))
            .When(x => x.Salary.HasValue);

        // Profile Picture URL validation
        RuleFor(x => x.ProfilePictureUrl)
            .Must(BeValidUrlOrEmpty)
            .WithMessage(localizationService.GetLocalizedString("InvalidProfilePictureUrl"))
            .When(x => !string.IsNullOrEmpty(x.ProfilePictureUrl));

        // Additional Interests validation
        RuleFor(x => x.AdditionalInterests)
            .Must(interests => interests == null || interests.Count <= 10)
            .WithMessage(localizationService.GetLocalizedString("TooManyAdditionalInterests"));

        RuleForEach(x => x.AdditionalInterests)
            .NotEmpty()
            .WithMessage(localizationService.GetLocalizedString("EmptyAdditionalInterest"))
            .Length(1, 100)
            .WithMessage(localizationService.GetLocalizedString("AdditionalInterestLength"))
            .When(x => x.AdditionalInterests != null);

        // Education validation
        RuleFor(x => x.Educations)
            .Must(educations => educations == null || educations.Count <= 10)
            .WithMessage(localizationService.GetLocalizedString("TooManyEducations"));

        RuleForEach(x => x.Educations).ChildRules(education =>
        {
            education.RuleFor(e => e.Institution)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("InstitutionRequired"))
                .Length(2, 200)
                .WithMessage(localizationService.GetLocalizedString("InstitutionLength"));

            education.RuleFor(e => e.Degree)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("DegreeRequired"))
                .Length(2, 100)
                .WithMessage(localizationService.GetLocalizedString("DegreeLength"));

            education.RuleFor(e => e.Field)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("FieldRequired"))
                .Length(2, 100)
                .WithMessage(localizationService.GetLocalizedString("FieldLength"));

            education.RuleFor(e => e.StartDate)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("StartDateRequired"))
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage(localizationService.GetLocalizedString("StartDateInFuture"));

            education.RuleFor(e => e.EndDate)
                .GreaterThan(e => e.StartDate)
                .WithMessage(localizationService.GetLocalizedString("EndDateAfterStartDate"))
                .When(e => e.EndDate.HasValue);

            education.RuleFor(e => e.Description)
                .MaximumLength(500)
                .WithMessage(localizationService.GetLocalizedString("DescriptionMaxLength"));
        })
        .When(x => x.Educations != null);

        // Certificate validation
        RuleFor(x => x.Certificates)
            .Must(certificates => certificates == null || certificates.Count <= 20)
            .WithMessage(localizationService.GetLocalizedString("TooManyCertificates"));

        RuleForEach(x => x.Certificates).ChildRules(certificate =>
        {
            certificate.RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("CertificateNameRequired"))
                .Length(2, 200)
                .WithMessage(localizationService.GetLocalizedString("CertificateNameLength"));

            certificate.RuleFor(c => c.IssuedBy)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("IssuedByRequired"))
                .Length(2, 200)
                .WithMessage(localizationService.GetLocalizedString("IssuedByLength"));

            certificate.RuleFor(c => c.IssuedDate)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("IssuedDateRequired"))
                .LessThanOrEqualTo(DateTime.Now)
                .WithMessage(localizationService.GetLocalizedString("IssuedDateInFuture"));

            certificate.RuleFor(c => c.CertificateUrl)
                .Must(BeValidUrlOrEmpty)
                .WithMessage(localizationService.GetLocalizedString("InvalidCertificateUrl"))
                .When(c => !string.IsNullOrEmpty(c.CertificateUrl));

            certificate.RuleFor(c => c.ExamResult)
                .MaximumLength(100)
                .WithMessage(localizationService.GetLocalizedString("ExamResultMaxLength"));
        })
        .When(x => x.Certificates != null);

        // Skills validation
        RuleFor(x => x.Skills)
            .Must(skills => skills == null || skills.Count <= 50)
            .WithMessage(localizationService.GetLocalizedString("TooManySkills"));

        RuleForEach(x => x.Skills).ChildRules(skill =>
        {
            skill.RuleFor(s => s.SkillId)
                .NotEmpty()
                .WithMessage(localizationService.GetLocalizedString("SkillIdRequired"));
        })
        .When(x => x.Skills != null);

        // Teaching preferences validation
        RuleFor(x => x.TeachingAreaIds)
            .Must(areas => areas == null || areas.Count <= 20)
            .WithMessage(localizationService.GetLocalizedString("TooManyTeachingAreas"));

        RuleFor(x => x.AgeGroupIds)
            .Must(groups => groups == null || groups.Count <= 10)
            .WithMessage(localizationService.GetLocalizedString("TooManyAgeGroups"));

        RuleFor(x => x.CommunicationMethodIds)
            .Must(methods => methods == null || methods.Count <= 10)
            .WithMessage(localizationService.GetLocalizedString("TooManyCommunicationMethods"));

        RuleFor(x => x.TeachingLanguageIds)
            .Must(languages => languages == null || languages.Count <= 10)
            .WithMessage(localizationService.GetLocalizedString("TooManyTeachingLanguages"));

        // Ensure no duplicate values in lists
        RuleFor(x => x.TeachingAreaIds)
            .Must(HaveUniqueValues)
            .WithMessage(localizationService.GetLocalizedString("DuplicateTeachingAreas"))
            .When(x => x.TeachingAreaIds != null && x.TeachingAreaIds.Any());

        RuleFor(x => x.AgeGroupIds)
            .Must(HaveUniqueValues)
            .WithMessage(localizationService.GetLocalizedString("DuplicateAgeGroups"))
            .When(x => x.AgeGroupIds != null && x.AgeGroupIds.Any());

        RuleFor(x => x.CommunicationMethodIds)
            .Must(HaveUniqueValues)
            .WithMessage(localizationService.GetLocalizedString("DuplicateCommunicationMethods"))
            .When(x => x.CommunicationMethodIds != null && x.CommunicationMethodIds.Any());

        RuleFor(x => x.TeachingLanguageIds)
            .Must(HaveUniqueValues)
            .WithMessage(localizationService.GetLocalizedString("DuplicateTeachingLanguages"))
            .When(x => x.TeachingLanguageIds != null && x.TeachingLanguageIds.Any());

        RuleFor(x => x.Skills)
            .Must(skills => skills == null || skills.Select(s => s.SkillId).Distinct().Count() == skills.Count)
            .WithMessage(localizationService.GetLocalizedString("DuplicateSkills"))
            .When(x => x.Skills != null && x.Skills.Any());
    }

    private static bool BeValidUrlOrEmpty(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
               (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }

    private static bool HaveUniqueValues<T>(IList<T>? values)
    {
        if (values == null) return true;
        return values.Distinct().Count() == values.Count;
    }
} 