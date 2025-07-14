using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Entities;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Teachers.Query.GetAll
{
    public class GetAllTeachersQueryHandler : IRequestHandler<GetAllTeachersQuery, Result<TeachersPagedResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;
        private readonly ICommentRepository _commentRepository;

        public GetAllTeachersQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService,
            ICommentRepository commentRepository)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
            _commentRepository = commentRepository;
        }

        public async Task<Result<TeachersPagedResult>> Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Get base query for teachers with includes
                var teachersQuery = await _unitOfWork.Repository<Teacher>()
                    .GetAllAsync();
                
                var baseQuery = teachersQuery
                    .Include(t => t.User)
                    .Include(t => t.Skills)
                        .ThenInclude(ts => ts.Skill)
                    .Include(t => t.Comments)
                    .Where(t => t.Status == Domain.Enums.Status.Accepted);

                // Apply skills filtering if provided
                if (request.SkillIds != null && request.SkillIds.Any())
                {
                    baseQuery = baseQuery.Where(t => 
                        t.Skills.Any(ts => request.SkillIds.Contains(ts.SkillId)));
                }

                // Get total count before pagination
                var totalCount = await baseQuery.CountAsync(cancellationToken);

                // Calculate pagination values
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var skip = (request.PageNumber - 1) * request.PageSize;

                // Get teachers for the current page
                var teachers = await baseQuery
                    .Skip(skip)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                // Build teacher DTOs with ratings
                var teacherDtos = new List<TeacherDto>();
                foreach (var teacher in teachers)
                {
                    // Get rating information
                    var averageRating = await _commentRepository.GetAverageRatingForTeacherAsync(teacher.Id);
                    var totalComments = await _commentRepository.GetCommentCountForTeacherAsync(teacher.Id);

                    var teacherDto = new TeacherDto
                    {
                        Id = teacher.Id,
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
                        Rating = new TeacherRatingInfoDto
                        {
                            AverageRating = Math.Round(averageRating, 1),
                            TotalComments = totalComments
                        }
                    };

                    teacherDtos.Add(teacherDto);
                }

                // Order by rating if requested (default)
                if (request.OrderByRating)
                {
                    teacherDtos = teacherDtos
                        .OrderByDescending(t => t.Rating.AverageRating)
                        .ThenByDescending(t => t.Rating.TotalComments)
                        .ToList();
                }

                // Create paged result
                var result = new TeachersPagedResult
                {
                    Teachers = teacherDtos,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasPreviousPage = request.PageNumber > 1,
                    HasNextPage = request.PageNumber < totalPages
                };

                return Result<TeachersPagedResult>.Success(result);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("TeacherGetAllFailed") + $": {ex.Message}";
                return Result<TeachersPagedResult>.Failure(error);
            }
        }
    }
}
