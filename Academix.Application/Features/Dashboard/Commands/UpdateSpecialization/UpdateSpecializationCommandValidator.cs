using FluentValidation;

namespace Academix.Application.Features.Dashboard.Commands.UpdateSpecialization;

public class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("SpecializationIdRequired");

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