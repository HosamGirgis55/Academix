using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Comments.Queries.GetTeacherComments;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Teachers.Query.GetById
{
    internal class GetTeacherByIdQueryHandler : IRequestHandler<GetTeacherByIdQuery, Result<TeacherDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly ICommentRepository _commentRepository;

        public GetTeacherByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            ICommentRepository commentRepository)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _commentRepository = commentRepository;
        }

        public async Task<Result<TeacherDto>> Handle(GetTeacherByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Use generic repository to get teacher with User navigation property
                var teacherQuery = await _unitOfWork.Repository<Teacher>()
                    .GetAllAsync();
                
                var teacher = await teacherQuery
                    .Include(t => t.User)
                    .Include(t => t.Skills)
                        .ThenInclude(ts => ts.Skill)
                    .Include(t => t.TeacherTeachingAreas)
                        .ThenInclude(tta => tta.TeachingArea)
                    .FirstOrDefaultAsync(t => t.Id == request.Id);

                if (teacher == null)
                {
                    var notFoundMsg = _localizationService.GetLocalizedString("TeacherNotFound");
                    return Result<TeacherDto>.Failure(notFoundMsg);
                }

                // Get comments for the teacher
                var comments = await _commentRepository.GetCommentsByTeacherIdAsync(request.Id);
                
                // Get teacher rating statistics
                var averageRating = await _commentRepository.GetAverageRatingForTeacherAsync(request.Id);
                var totalComments = await _commentRepository.GetCommentCountForTeacherAsync(request.Id);

                var teacherRating = new TeacherRatingInfoDto
                {
                    AverageRating = Math.Round(averageRating, 1),
                    TotalComments = totalComments
                };

                // Map comments to DTOs (safe even if comments list is empty)
                var commentDtos = comments.Select(comment => new TeacherCommentDto
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    Rating = comment.Rating,
                    CreatedAt = comment.CreatedAt,
                    Student = new TeacherStudentInfoDto
                    {
                        Id = comment.Student.Id,
                        FirstName = comment.Student.User.FirstName,
                        LastName = comment.Student.User.LastName,
                        ProfilePictureUrl = comment.Student.User.ProfilePictureUrl ?? ""
                    }
                }).ToList();

                var teacherDto = new TeacherDto
                {
                    Id = teacher.Id,
                    userId = teacher.UserId,
                    FirstName = teacher.User.FirstName,
                    LastName = teacher.User.LastName,
                    Bio = teacher.Bio,
                    ProfilePictureUrl = teacher.ProfilePictureUrl,
                    Salary = teacher.Salary,
                    Skills = teacher.Skills?.Select(s => new TeacherSkillDto
                    {
                        SkillId = s.SkillId,
                        SkillName = s.Skill.NameAr
                    }).ToList() ?? new List<TeacherSkillDto>(),
                    Specialists = teacher.TeacherTeachingAreas?.Select(tta => new TeacherSpecialistDto
                    {
                        TeachingAreaId = tta.TeachingAreaId,
                        NameAr = tta.TeachingArea.NameAr,
                        NameEn = tta.TeachingArea.NameEn
                    }).ToList() ?? new List<TeacherSpecialistDto>(),
                    Rating = new TeacherRatingInfoDto
                    {
                        AverageRating = Math.Round(averageRating, 1),
                        TotalComments = totalComments
                    }
                };

                return Result<TeacherDto>.Success(teacherDto);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("TeacherGetByIdFailed") + $": {ex.Message}";
                return Result<TeacherDto>.Failure(error);
            }
        }
    }
}
