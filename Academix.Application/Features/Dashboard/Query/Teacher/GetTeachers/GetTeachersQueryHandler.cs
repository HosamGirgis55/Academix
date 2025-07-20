using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Query.GetAll;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using Academix.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Query.Teacher.GetTeachers
{
    internal class GetTeachersQueryHandler : IRequestHandler<GetTeacherQuery, Result<TeachersPagedResult>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public GetTeachersQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result<TeachersPagedResult>> Handle(GetTeacherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate pagination parameters
                if (request.PageNumber < 1)
                    request.PageNumber = 1;
                
                if (request.PageSize < 1)
                    request.PageSize = 10;
                
                if (request.PageSize > 100)
                    request.PageSize = 100;

                var teachers = await _unitOfWork.Repository<Domain.Entities.Teacher>()
                    .GetAllAsync();
                teachers = teachers.Include(x => x.User)
                    .Include(t=>t.Skills)
                        .ThenInclude(ts => ts.Skill)
                    .Include(t=>t.Certificates)
                    .Include(t => t.TeacherTeachingAreas)
                        .ThenInclude(tta => tta.TeachingArea)
                    .Include(t=>t.Educations);


                // Apply status filtering
                var filteredTeachers = teachers.Where(t => t.Status == request.Status).ToList();

                // Get total count after filtering
                var totalCount = filteredTeachers.Count;

                if (totalCount == 0)
                {
                    var notFoundMsg = _localizationService.GetLocalizedString("empty");
                    return Result<TeachersPagedResult>.Failure(notFoundMsg);
                }

                // Calculate pagination values
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var skipCount = (request.PageNumber - 1) * request.PageSize;

                // Apply pagination
                var paginatedTeachers = filteredTeachers
                    .Skip(skipCount)
                    .Take(request.PageSize)
                    .ToList();

                var teacherDtos = paginatedTeachers.Select(t => new TeacherDto
                {
                    Id = t.Id,
                    userId = t.UserId,
                    Email = t.User.Email,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    Bio = t.Bio,
                    ProfilePictureUrl = t.ProfilePictureUrl,
                    Salary = t.Salary,
                    Skills = t.Skills?.Select(s => new TeacherSkillDto
                    {
                        SkillId = s.SkillId,
                        SkillName = s.Skill.NameAr
                    }).ToList() ?? new List<TeacherSkillDto>(),
                    Specialists = t.TeacherTeachingAreas?.Select(tta => new TeacherSpecialistDto
                    {
                        TeachingAreaId = tta.TeachingAreaId,
                        NameAr = tta.TeachingArea.NameAr,
                        NameEn = tta.TeachingArea.NameEn
                    }).ToList() ?? new List<TeacherSpecialistDto>(),
                    Certificates = t.Certificates?.Select(c => new CertificatesDto
                    {
                        Name = c.Name,
                        CertificateUrl = c.CertificateUrl,
                        ExamResult = c.ExamResult,
                        IssuedBy = c.IssuedBy,
                        IssuedDate = c.IssuedDate,
                    }).ToList() ?? new List<CertificatesDto>(),
                    teacherEducations = t.Educations?.ToList() ?? new List<TeacherEducation>(),
                    stutas = t.Status
                }).ToList();

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
