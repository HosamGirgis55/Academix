using Academix.Application.Common.Interfaces;
using Academix.Application.Common.Models;
using Academix.Domain.DTOs;
using Academix.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Academix.Application.Features.Teachers.Query.GetAll
{
    internal class GetAllTeachersQueryHandler : IRequestHandler<GetAllTeachersQuery, Result<List<TeacherDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationService _localizationService;

        public GetAllTeachersQueryHandler(
            IUnitOfWork unitOfWork,
            ILocalizationService localizationService)
        {
            _unitOfWork = unitOfWork;
            _localizationService = localizationService;
        }

        public async Task<Result<List<TeacherDto>>> Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var teachers = await _unitOfWork.Teachers.GetAllAsync();

                var teacherList = teachers
                    .Select(t => new TeacherDto
                    {
                        Id = t.Id,
                        FirstName = t.User.FirstName,
                        LastName = t.User.LastName,
                        Bio = t.Bio,
                        ProfilePictureUrl = t.ProfilePictureUrl,
                        //Salary = t.Salary,
                        //Skills = t.Skills.Select(s => s.Name).ToList()
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
