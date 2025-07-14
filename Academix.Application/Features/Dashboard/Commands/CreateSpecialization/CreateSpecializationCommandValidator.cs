using FluentValidation;

namespace Academix.Application.Features.Dashboard.Commands.CreateSpecialization;

public class CreateSpecializationCommandValidator : AbstractValidator<CreateSpecializationCommand>
{
    public CreateSpecializationCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty()
            .WithMessage("SpecializationNameArRequired")
            .MinimumLength(2)
            .WithMessage("SpecializationNameArMinLength")
            .MaximumLength(100)
            .WithMessage("SpecializationNameArMaxLength");

        RuleFor(x => x.NameEn)
            .NotEmpty()
            .WithMessage("SpecializationNameEnRequired")
            .MinimumLength(2)
            .WithMessage("SpecializationNameEnMinLength")
            .MaximumLength(100)
            .WithMessage("SpecializationNameEnMaxLength");
    }
} 