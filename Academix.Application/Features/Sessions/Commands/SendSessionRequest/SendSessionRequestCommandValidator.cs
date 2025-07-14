using Academix.Application.Common.Interfaces;
using FluentValidation;

namespace Academix.Application.Features.Sessions.Commands.SendSessionRequest
{
    public class SendSessionRequestCommandValidator : AbstractValidator<SendSessionRequestCommand>
    {
        private readonly ILocalizationService _localizationService;

        public SendSessionRequestCommandValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("StudentIdRequired"));

            RuleFor(x => x.TeacherId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("TeacherIdRequired"));

            RuleFor(x => x.PointsAmount)
                .GreaterThan(0).WithMessage(_localizationService.GetLocalizedString("PointsAmountMustBePositive"))
                .LessThanOrEqualTo(1000).WithMessage(_localizationService.GetLocalizedString("PointsAmountTooHigh"));

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("SubjectRequired"))
                .MinimumLength(3).WithMessage(_localizationService.GetLocalizedString("SubjectMinLength"))
                .MaximumLength(100).WithMessage(_localizationService.GetLocalizedString("SubjectMaxLength"));

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("DescriptionRequired"))
                .MinimumLength(10).WithMessage(_localizationService.GetLocalizedString("DescriptionMinLength"))
                .MaximumLength(500).WithMessage(_localizationService.GetLocalizedString("DescriptionMaxLength"));

            RuleFor(x => x.EstimatedDurationMinutes)
                .GreaterThan(15).WithMessage(_localizationService.GetLocalizedString("MinimumSessionDuration"))
                .LessThanOrEqualTo(480).WithMessage(_localizationService.GetLocalizedString("MaximumSessionDuration"));

            RuleFor(x => x.RequestedDateTime)
                .GreaterThan(DateTime.Now).WithMessage(_localizationService.GetLocalizedString("RequestedDateTimeMustBeFuture"));
        }
    }
} 