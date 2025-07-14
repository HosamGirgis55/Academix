using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Application.Features.Teachers.Query.GetAll;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academix.Application.Features.Dashboard.Query.Teacher.GetTeachers
{
    internal class GetTeachersQueryHandler : IRequestHandler<GetTeacherQuery, Result<List<TeacherDto>>>
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

        public async Task<Result<List<TeacherDto>>> Handle(GetTeacherQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var teachers = await _unitOfWork.Teachers.GetAllAsync();

                var teacherList = teachers.Where(t => (t.Status == request.Status /*&&.Field == request.Field*/))
                    .Select(t => new TeacherDto
                    {
                        Id = t.Id,
                        FirstName = t.User.FirstName,
                        LastName = t.User.LastName,
                        Bio = t.Bio,
                        ProfilePictureUrl = t.ProfilePictureUrl,
                        Salary = t.Salary,
                        Skills = t.Skills.Select(s => new TeacherSkillDto
                        {
                            SkillName = s.Skill.NameAr
                        }).ToList(),
                        stutas = t.Status
                    }).ToList();

                return Result<List<TeacherDto>>.Success(teacherList);
            }
            catch (Exception ex)
            {
                var error = _localizationService.GetLocalizedString("TeacherGetAllFailed") + $": {ex.Message}";
                return Result<List<TeacherDto>>.Failure(error);
            }
        }
    }
}
