using Academix.Application.Common.Interfaces;
using FluentValidation;
using System;

namespace Academix.Application.Features.Comments.Commands.AddComment
{
    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        private readonly ILocalizationService _localizationService;

        public AddCommentCommandValidator(ILocalizationService localizationService)
        {
            _localizationService = localizationService;

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("CommentContentRequired"))
                .MinimumLength(10).WithMessage(_localizationService.GetLocalizedString("CommentContentMinLength"))
                .MaximumLength(1000).WithMessage(_localizationService.GetLocalizedString("CommentContentMaxLength"));

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage(_localizationService.GetLocalizedString("RatingRange"));

            RuleFor(x => x.TeacherId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("TeacherIdRequired"));

            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage(_localizationService.GetLocalizedString("StudentIdRequired"));
        }
    }
} 