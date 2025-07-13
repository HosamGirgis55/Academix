using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;

namespace Academix.Application.Features.Comments.Queries.GetTeacherComments
{
    public class GetTeacherCommentsQueryHandler : IRequestHandler<GetTeacherCommentsQuery, Result<List<CommentDto>>>
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public GetTeacherCommentsQueryHandler(
            ICommentRepository commentRepository,
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result<List<CommentDto>>> Handle(GetTeacherCommentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if teacher exists
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
                if (teacher == null)
                {
                    return Result<List<CommentDto>>.Failure(_localizationService.GetLocalizedString("TeacherNotFound"));
                }

                // Get comments for the teacher
                var comments = await _commentRepository.GetCommentsByTeacherIdAsync(request.TeacherId);
                
                // Get teacher rating statistics
                var averageRating = await _commentRepository.GetAverageRatingForTeacherAsync(request.TeacherId);
                var totalComments = await _commentRepository.GetCommentCountForTeacherAsync(request.TeacherId);

                var teacherRating = new TeacherRatingDto
                {
                    AverageRating = Math.Round(averageRating, 1),
                    TotalComments = totalComments
                };

                // Map to DTOs
                var commentDtos = comments.Select(comment => new CommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    Rating = comment.Rating,
                    CreatedAt = comment.CreatedAt,
                    Student = new StudentInfoDto
                    {
                        Id = comment.Student.Id,
                        FirstName = comment.Student.User.FirstName,
                        LastName = comment.Student.User.LastName,
                        ProfilePictureUrl = comment.Student.User.ProfilePictureUrl ?? ""
                    },
                    TeacherRating = teacherRating
                }).ToList();

                return Result<List<CommentDto>>.Success(commentDtos);
            }
            catch (Exception ex)
            {
                return Result<List<CommentDto>>.Failure($"{_localizationService.GetLocalizedString("GetCommentsFailed")}: {ex.Message}");
            }
        }
    }
} 