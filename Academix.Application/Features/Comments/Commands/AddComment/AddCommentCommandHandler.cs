using System;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Comments.Commands.AddComment
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICommentRepository _commentRepository;
        private readonly ILocalizationService _localizationService;

        public AddCommentCommandHandler(
            IUnitOfWork unitOfWork,
            ICommentRepository commentRepository,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _commentRepository = commentRepository;
            _localizationService = localizationService;
        }

        public async Task<Result> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if student exists
                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(request.StudentId);
                if (student == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("StudentNotFound"));
                }

                // Check if teacher exists
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
                if (teacher == null)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("TeacherNotFound"));
                }

                // Check if student has already commented on this teacher
                var hasCommented = await _commentRepository.HasStudentCommentedOnTeacherAsync(
                    request.StudentId, request.TeacherId);
                
                if (hasCommented)
                {
                    return Result.Failure(_localizationService.GetLocalizedString("AlreadyCommented"));
                }

                // Create new comment
                var comment = new Comment
                {
                    Content = request.Content,
                    Rating = request.Rating,
                    StudentId = request.StudentId,
                    TeacherId = request.TeacherId
                };

                await _unitOfWork.Repository<Comment>().AddAsync(comment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success(_localizationService.GetLocalizedString("CommentAddedSuccessfully"));
            }
            catch (Exception ex)
            {
                return Result.Failure($"{_localizationService.GetLocalizedString("CommentAddFailed")}: {ex.Message}");
            }
        }
    }
} 